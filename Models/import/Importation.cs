using System.Data.Common;
using System.Text;
using Npgsql;

namespace Evaluation.Models
{
    public class Importation
    {
        public static void DeleteErrorInTempTable<T>(NpgsqlConnection connection,
        NpgsqlTransaction transaction, List<T> temps) where T : StdTemp
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                foreach (var item in temps)
                {
                    item.ExecuteNonQuery(connection, transaction, @$"DELETE FROM std_temp WHERE 
                    row = {item.Row}");
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (isNewConnexion == true)
                {
                    connection.Close();
                }
            }
        }

        public static string CombineErrorsIntoOne<T>(List<T> temps) where T : StdTemp
        {
            StringBuilder combined = new StringBuilder();

            foreach (var temp in temps)
            {
                combined.Append(temp.Error + "\\n");
            }
            return combined.ToString();
        }

        public static void ImportPaiementIntoDatabase(NpgsqlConnection connection, string filePath)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            NpgsqlTransaction transaction = connection.BeginTransaction();
            try
            {
                Console.WriteLine("CSV");
                CreateTempTable(connection, transaction, filePath);
                using (var reader = new StreamReader(filePath))
                {
                    string headerLine = reader.ReadLine();
                    using (var writer = connection.BeginTextImport($"COPY std_temp({headerLine.Replace(";", ",")}) FROM STDIN CSV DELIMITER ','"))
                    {
                        writer.Write(reader.ReadToEnd());
                    }
                }
                List<StdTempPaiement> errors = ValidateCSV<StdTempPaiement>(connection, transaction, new StdTempPaiement());
                if (errors.Count > 0)
                {
                    DeleteErrorInTempTable<StdTempPaiement>(connection, transaction, errors);
                    //throw new Exception(CombineErrorsIntoOne<StdTempPaiement>(errors));
                }
                InsertIntoPaiementFromTemp(connection, transaction);
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                transaction.Dispose();
                if (isNewConnexion == true)
                {
                    connection.Close();
                }
            }
        }

        public static List<T> ValidateCSV<T>(NpgsqlConnection connection, DbTransaction transaction, StdTemp std) where T : StdTemp
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                List<T> temps = new StdTemp().Find<T>(connection, "");
                List<T> errors = new List<T>();

                foreach (var temp in temps)
                {
                    temp.Validate(connection);
                    string error = temp.Error;

                    if (!string.IsNullOrEmpty(error))
                    {
                        errors.Add(temp);
                    }
                }
                return errors;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (isNewConnexion == true)
                {
                    connection.Close();
                }
            }
        }

        public static void InsertIntoPaiementFromTemp(DbConnection connection, DbTransaction transaction)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                // Console.WriteLine("Nouvelle connexion");
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.Connection = connection;
                    if (transaction != null)
                    {
                        command.Transaction = transaction;
                    }
                    string query = $"INSERT INTO paiement " +
                        "(iddevis, ref_paiement, date, montant) " +
                    "SELECT devis.id AS iddevis, ref_paiement, CAST(date_paiement AS TIMESTAMP) AS date, CAST(REPLACE(montant, ',', '.') AS DOUBLE PRECISION) AS montant " +
                    "FROM std_temp " +
                    "JOIN devis ON devis.numero = std_temp.ref_devis;";
                    Console.WriteLine(query);
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                if (isNewConnexion == true)
                {
                    connection.Close();
                }
            }
        }

        public static void ImportMaisonTravauxIntoDatabase(NpgsqlConnection connection, string filePath)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            NpgsqlTransaction transaction = connection.BeginTransaction();
            try
            {
                CreateTempTable(connection, transaction, filePath);
                using (var reader = new StreamReader(filePath))
                {
                    string headerLine = reader.ReadLine();
                    using (var writer = connection.BeginTextImport(@$"COPY std_temp({headerLine.Replace(";", ",")}) FROM STDIN CSV DELIMITER ','"))
                    {
                        writer.Write(reader.ReadToEnd());
                    }
                }
                // List<string> errors = ValidateCSV<StdTempMaisonTravaux>(connection, transaction, new StdTempMaisonTravaux());
                // if (errors.Count > 0)
                // {
                //     throw new Exception(string.Join("<br>", errors));
                // }
                InsertIntoUniteFromTemp(connection, transaction);
                InsertIntoTypeMaisonFromTemp(connection, transaction);
                InsertIntoTypeTravauxFromTemp(connection, transaction);
                InsertIntoTravauxTypeDeMaisonFromTemp(connection, transaction);
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                transaction.Dispose();
                if (isNewConnexion == true)
                {
                    connection.Close();
                }
            }
        }

        public static void InsertIntoTravauxTypeDeMaisonFromTemp(DbConnection connection, DbTransaction transaction)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                // Console.WriteLine("Nouvelle connexion");
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.Connection = connection;
                    if (transaction != null)
                    {
                        command.Transaction = transaction;
                    }
                    string query = $"INSERT INTO travauxTypeDeMaison " +
                        "(idTypeMaison, idTypeTravaux, quantite) " +
                    "SELECT typeMaison.id AS idTypeMaison, typeTravaux.id AS idTypeTravaux, CAST(REPLACE(quantite, ',', '.') AS DOUBLE PRECISION) AS quantite " +
                    "FROM std_temp " +
                    "JOIN typeMaison ON typeMaison.nom = std_temp.type_maison " +
                    "JOIN typeTravaux ON typeTravaux.numero = std_temp.code_travaux; ";
                    Console.WriteLine(query);
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                if (isNewConnexion == true)
                {
                    connection.Close();
                }
            }
        }

        public static void InsertIntoTypeTravauxFromTemp(DbConnection connection, DbTransaction transaction)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                // Console.WriteLine("Nouvelle connexion");
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.Connection = connection;
                    if (transaction != null)
                    {
                        command.Transaction = transaction;
                    }
                    string query = $"INSERT INTO typeTravaux " +
                        "(numero, designation, idunite, prixUnitaire) " +
                    "SELECT code_travaux AS numero, type_travaux AS designation, unite.id AS idunite, CAST(REPLACE(prix_unitaire, ',', '.') AS DOUBLE PRECISION) AS prixUnitaire " +
                    "FROM std_temp " +
                    "JOIN unite ON unite.nom = std_temp.unite " +
                    "GROUP BY code_travaux, type_travaux, unite.id, prix_unitaire;";
                    Console.WriteLine(query);
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                if (isNewConnexion == true)
                {
                    connection.Close();
                }
            }
        }

        public static void InsertIntoUniteFromTemp(DbConnection connection, DbTransaction transaction)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                // Console.WriteLine("Nouvelle connexion");
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.Connection = connection;
                    if (transaction != null)
                    {
                        command.Transaction = transaction;
                    }
                    string query = $"INSERT INTO unite " +
                        "(nom) " +
                    "SELECT unite AS nom " +
                    "FROM std_temp " +
                    "GROUP BY unite;";
                    Console.WriteLine(query);
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                if (isNewConnexion == true)
                {
                    connection.Close();
                }
            }
        }

        public static void InsertIntoTypeMaisonFromTemp(DbConnection connection, DbTransaction transaction)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                // Console.WriteLine("Nouvelle connexion");
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.Connection = connection;
                    if (transaction != null)
                    {
                        command.Transaction = transaction;
                    }
                    string query = $"INSERT INTO typeMaison " +
                        "(nom, description, dureedeconstruction, surface) " +
                    "SELECT type_maison AS nom, description, CAST(duree_travaux AS INT) AS dureedeconstruction, CAST(REPLACE(surface, ',', '.') AS DOUBLE PRECISION) AS surface " +
                    "FROM std_temp " +
                    "GROUP BY type_maison, description, duree_travaux, surface;";
                    Console.WriteLine(query);
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                if (isNewConnexion == true)
                {
                    connection.Close();
                }
            }
        }

        public static void ImportDevisIntoDatabase(NpgsqlConnection connection, string filePath)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            NpgsqlTransaction transaction = connection.BeginTransaction();
            try
            {
                Console.WriteLine("CSV");
                CreateTempTable(connection, transaction, filePath);
                using (var reader = new StreamReader(filePath))
                {
                    string headerLine = reader.ReadLine();
                    using (var writer = connection.BeginTextImport(@$"COPY std_temp({headerLine.Replace(";", ",")}) FROM STDIN CSV DELIMITER ','"))
                    {
                        writer.Write(reader.ReadToEnd());
                    }
                }
                // List<string> errors = ValidateCSV<StdTempDevis>(connection, transaction, new StdTempDevis());
                // if (errors.Count > 0)
                // {
                //     throw new Exception(string.Join("<br>", errors));
                // }
                InsertIntoClientFromTemp(connection, transaction);
                InsertIntoTypeFinitionFromTemp(connection, transaction);
                InsertIntoDevisFromTemp(connection, transaction);
                InsertIntoTravauxDevisFromTemp(connection, transaction);
                UpdateDevisFromTravauxDevis(connection, transaction);
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                transaction.Dispose();
                if (isNewConnexion == true)
                {
                    connection.Close();
                }
            }
        }

        public static void UpdateDevisFromTravauxDevis(DbConnection connection, DbTransaction transaction)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                // Console.WriteLine("Nouvelle connexion");
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.Connection = connection;
                    if (transaction != null)
                    {
                        command.Transaction = transaction;
                    }
                    string query = @$"UPDATE devis
                    SET montanttravaux = (
                        SELECT SUM(quantite * prixUnitaire) FROM travauxDevis 
                        WHERE travauxDevis.iddevis = devis.id
                    )";
                    Console.WriteLine(query);
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                if (isNewConnexion == true)
                {
                    connection.Close();
                }
            }
        }

        public static void InsertIntoTravauxDevisFromTemp(DbConnection connection, DbTransaction transaction)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                // Console.WriteLine("Nouvelle connexion");
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.Connection = connection;
                    if (transaction != null)
                    {
                        command.Transaction = transaction;
                    }
                    string query = @$"INSERT INTO travauxDevis (idDevis, idTypeTravaux, quantite, 
                    prixUnitaire)
                    SELECT devis.id AS iddevis, typeTravaux.id AS idTypeTravaux, travauxTypeDeMaison.quantite, typeTravaux.prixUnitaire
                    FROM devis
                    JOIN travauxTypeDeMaison ON travauxTypeDeMaison.idtypemaison = devis.idtypemaison
                    JOIN typeTravaux ON travauxTypeDeMaison.idTypeTravaux = typeTravaux.id;";
                    Console.WriteLine(query);
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                if (isNewConnexion == true)
                {
                    connection.Close();
                }
            }
        }

        public static void InsertIntoDevisFromTemp(DbConnection connection, DbTransaction transaction)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                // Console.WriteLine("Nouvelle connexion");
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.Connection = connection;
                    if (transaction != null)
                    {
                        command.Transaction = transaction;
                    }
                    string query = @$"INSERT INTO devis 
                        (numero, idclient, idtypemaison, dureedeconstruction, idtypefinition, pourcentagefinition, datedevis, datedebut, lieu) 
                    SELECT ref_devis, clients.id AS idclient, typemaison.id AS idtypemaison, 
                    typemaison.dureedeconstruction, typefinition.id AS idtypefinition,
                    CAST(REPLACE(REPLACE(taux_finition, ',', '.'), '%', '') AS DOUBLE PRECISION) AS pourcentagefinition, 
                    CAST(date_devis AS TIMESTAMP) AS datedevis, CAST(date_debut AS TIMESTAMP) AS datedebut, lieu
                    FROM std_temp 
                    JOIN clients ON clients.numero = std_temp.client 
                    JOIN typemaison ON typemaison.nom = std_temp.type_maison 
                    JOIN typefinition ON typefinition.nom = std_temp.finition;";
                    Console.WriteLine(query);
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                if (isNewConnexion == true)
                {
                    connection.Close();
                }
            }
        }

        public static void InsertIntoTypeFinitionFromTemp(DbConnection connection, DbTransaction transaction)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                // Console.WriteLine("Nouvelle connexion");
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.Connection = connection;
                    if (transaction != null)
                    {
                        command.Transaction = transaction;
                    }
                    string query = $"INSERT INTO typeFinition " +
                        "(nom, pourcentage) " +
                    "SELECT finition AS nom, CAST(REPLACE(REPLACE(taux_finition, ',', '.'), '%', '') AS DOUBLE PRECISION) AS pourcentage " +
                    "FROM std_temp " +
                    "GROUP BY finition, taux_finition;";
                    Console.WriteLine(query);
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                if (isNewConnexion == true)
                {
                    connection.Close();
                }
            }
        }

        public static void InsertIntoClientFromTemp(DbConnection connection, DbTransaction transaction)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                // Console.WriteLine("Nouvelle connexion");
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.Connection = connection;
                    if (transaction != null)
                    {
                        command.Transaction = transaction;
                    }
                    string query = $"INSERT INTO clients " +
                        "(numero) " +
                    "SELECT client AS numero " +
                    "FROM std_temp " +
                    "GROUP BY client;";
                    Console.WriteLine(query);
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                if (isNewConnexion == true)
                {
                    connection.Close();
                }
            }
        }

        public static string CsvColumnToSQL(List<string> colonnes)
        {
            //Console.WriteLine(colonnes.Count);
            string sqlTableColumn = "row SERIAL,";
            foreach (var item in colonnes)
            {
                //Console.WriteLine(item);
                sqlTableColumn += item + " VARCHAR(255),";
            }
            return sqlTableColumn.Substring(0, sqlTableColumn.Length - 1);
        }

        public static void CreateTempTable(DbConnection connection, DbTransaction transaction, string filePath)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                Console.WriteLine("Nouvelle connexion");
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                CsvColumnReader ccr = new CsvColumnReader();
                List<string> colonnes = ccr.GetColumnNames(filePath).ToList();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.Connection = connection;
                    if (transaction != null)
                    {
                        command.Transaction = transaction;
                    }
                    string query = $"CREATE TEMP TABLE std_temp ({CsvColumnToSQL(colonnes)})";
                    Console.WriteLine(query);
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                if (isNewConnexion == true)
                {
                    connection.Close();
                }
            }
        }
    }
}