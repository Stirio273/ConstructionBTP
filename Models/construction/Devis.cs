using System.Globalization;
using Npgsql;

namespace Evaluation.Models
{
    public class Devis : BDDObject
    {
        int id;
        string numero;
        int idClient;
        int idTypeMaison;
        int dureeDeConstruction;
        double surface;
        int idTypeFinition;
        double pourcentageFinition;
        DateTime dateDevis;
        DateTime dateDebut;
        double montantTravaux;
        double paiementEffectue;
        TypeMaison typeMaison;
        TypeFinition typeFinition;
        string lieu;

        public Devis()
        {
        }

        public Devis(int id, string numero, int idClient, int idTypeMaison, DateTime dateDebut)
        {
            Id = id;
            Numero = numero;
            IdClient = idClient;
            IdTypeMaison = idTypeMaison;
            DateDebut = dateDebut;
        }

        public Devis(int id, string numero, int idClient, TypeMaison typeMaison,
        TypeFinition typeFinition, DateTime dateDebut, double montantTravaux)
        {
            Id = id;
            Numero = numero;
            IdClient = idClient;
            TypeMaison = typeMaison;
            TypeFinition = typeFinition;
            DateDebut = dateDebut;
            DureeDeConstruction = TypeMaison.DureeDeConstruction;
            PourcentageFinition = TypeFinition.Pourcentage;
            MontantTravaux = montantTravaux;
        }

        public Devis(int id, string numero, int idClient, TypeMaison typeMaison,
        TypeFinition typeFinition, DateTime dateDebut, double montantTravaux, double paiementEffectue)
        {
            Id = id;
            Numero = numero;
            IdClient = idClient;
            TypeMaison = typeMaison;
            TypeFinition = typeFinition;
            DateDebut = dateDebut;
            DureeDeConstruction = TypeMaison.DureeDeConstruction;
            PourcentageFinition = TypeFinition.Pourcentage;
            MontantTravaux = montantTravaux;
            PaiementEffectue = paiementEffectue;
        }

        [Identity]
        public int Id { get => id; set => id = value; }
        [Default]
        public string Numero { get => numero; set => numero = value; }
        public int IdClient { get => idClient; set => idClient = value; }
        public int IdTypeMaison
        {
            get => idTypeMaison;
            set
            {
                idTypeMaison = value;
                if (TypeMaison == null)
                {
                    TypeMaison = new TypeMaison(idTypeMaison, "", "", 0);
                    return;
                }
                TypeMaison.Id = idTypeMaison;
            }
        }
        public DateTime DateDebut { get => dateDebut; set => dateDebut = value; }
        public int DureeDeConstruction { get => dureeDeConstruction; set => dureeDeConstruction = value; }
        public double PourcentageFinition { get => pourcentageFinition; set => pourcentageFinition = value; }
        public int IdTypeFinition
        {
            get => idTypeFinition;
            set
            {
                idTypeFinition = value;
                if (TypeFinition == null)
                {
                    TypeFinition = new TypeFinition(idTypeFinition, "", 0);
                    return;
                }
                TypeFinition.Id = idTypeFinition;
            }
        }
        public TypeMaison TypeMaison
        {
            get => typeMaison;
            set
            {
                typeMaison = value;
                idTypeMaison = typeMaison.Id;
            }
        }
        public TypeFinition TypeFinition
        {
            get => typeFinition;
            set
            {
                typeFinition = value;
                idTypeFinition = typeFinition.Id;
            }
        }

        public double MontantTravaux { get => montantTravaux; set => montantTravaux = value; }
        public string MontantTravauxString
        {
            get
            {
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberGroupSeparator = " ";
                return MontantTravaux.ToString("N2", nfi);
            }
        }
        public string Lieu { get => lieu; set => lieu = value; }
        [Default]
        public DateTime DateDevis { get => dateDevis; set => dateDevis = value; }
        public double Surface { get => surface; set => surface = value; }
        public double PaiementEffectue { get => paiementEffectue; set => paiementEffectue = value; }
        public string PaiementEffectueString
        {
            get
            {
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberGroupSeparator = " ";
                return PaiementEffectue.ToString("N2", nfi);
            }
        }

        public DateTime GetDateFin()
        {
            return DateDebut.AddDays(DureeDeConstruction);
        }

        public double GetMontantTotal()
        {
            return this.MontantTravaux * (1 + (this.PourcentageFinition / 100));
        }

        public string ShowMontantTotal()
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberGroupSeparator = " ";
            return this.GetMontantTotal().ToString("N2", nfi);
        }

        public string ShowPourcentagePaiementEffectue()
        {
            return Formatter.FormatDouble((this.PaiementEffectue / this.GetMontantTotal()) * 100);
        }

        public override string TableName()
        {
            return "devis";
        }

        public string GetColor()
        {
            if (((this.PaiementEffectue / this.GetMontantTotal()) * 100) < 50)
            {
                return "badge badge-danger";
            }
            else if (((this.PaiementEffectue / this.GetMontantTotal()) * 100) == 50)
            {
                return "";
            }
            return "badge badge-success";
        }

        public List<Devis> GetAllDevis(NpgsqlConnection connection, string filtre)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                List<Devis> listeDevis = new List<Devis>();
                string query = $"SELECT * FROM vw_devis {filtre}";
                using (var command = connection.CreateCommand())
                {
                    command.Parameters.AddWithValue("idClient", this.Id);
                    command.CommandText = query;
                    command.Connection = connection;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listeDevis.Add(new Devis(reader.GetInt32(reader.GetOrdinal("id")),
                            reader.GetString(reader.GetOrdinal("numero")), this.Id,
                            new TypeMaison(reader.GetInt32(reader.GetOrdinal("idTypeMaison")),
                            reader.GetString(reader.GetOrdinal("nomMaison")), "",
                            reader.GetInt32(reader.GetOrdinal("dureeDeConstruction"))),
                             new TypeFinition(reader.GetInt32(reader.GetOrdinal("idTypeFinition")),
                             reader.GetString(reader.GetOrdinal("nomFinition")), reader.GetDouble(reader.GetOrdinal("pourcentageFinition"))),
                             reader.GetDateTime(reader.GetOrdinal("dateDebut")), reader.GetDouble(reader.GetOrdinal("montantTravaux"))));
                        }
                    }
                }
                return listeDevis;
            }
            catch (System.Exception)
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

        public List<TravauxDevis> GetAllTravaux(NpgsqlConnection connection, string filtre)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                List<TravauxDevis> listeTravaux = new List<TravauxDevis>();
                string query = $"SELECT * FROM vw_travauxDevis WHERE idTypeMaison = @idTypeMaison AND iddevis = @iddevis {filtre}";
                using (var command = connection.CreateCommand())
                {
                    command.Parameters.AddWithValue("idTypeMaison", this.IdTypeMaison);
                    command.Parameters.AddWithValue("iddevis", this.Id);
                    command.CommandText = query;
                    command.Connection = connection;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listeTravaux.Add(new TravauxDevis(reader.GetInt32(reader.GetOrdinal("id")), this.Id,
                            reader.GetDouble(reader.GetOrdinal("quantite")),
                            reader.GetDouble(reader.GetOrdinal("prixunitaire")),
                             new TypeTravaux(reader.GetInt32(reader.GetOrdinal("idTypeTravaux")),
                            reader.GetString(reader.GetOrdinal("numero")),
                            reader.GetString(reader.GetOrdinal("designation")),
                            new Unite(0, reader.GetString(reader.GetOrdinal("unite")))), reader.GetDouble(reader.GetOrdinal("total"))));
                        }
                    }
                }
                return listeTravaux;
            }
            catch (System.Exception)
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

        public double GetMontantRestant(NpgsqlConnection connection)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                return Convert.ToDouble(this.ExecuteQuery(connection,
                @$"SELECT getMontantRestant({this.Id})").Rows[0][0]);
            }
            catch (System.Exception)
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

        public void Payer(NpgsqlConnection connection, Paiement paiement)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                // if (paiement.Montant > this.GetMontantRestant(connection))
                // {
                //     throw new Exception("Le montant total des paiements depasse le montant total du devis");
                // }
                // paiement.Insert(connection, null);
                this.ExecuteNonQuery(connection, null,
                @$"SELECT payer({this.Id}, {paiement.Montant}, '{paiement.Date}')");
            }
            catch (System.Exception)
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

        // public List<Devis> GetAllDevis(NpgsqlConnection connection)
        // {
        //     bool isNewConnexion = false;
        //     if (connection == null)
        //     {
        //         connection = new Connect().getConnectPostgres();
        //         isNewConnexion = true;
        //     }
        //     try
        //     {
        //         List<Devis> listeDevis = new List<Devis>();
        //         string query = "SELECT * FROM vw_devis";
        //         using (var command = connection.CreateCommand())
        //         {
        //             command.CommandText = query;
        //             command.Connection = connection;
        //             using (var reader = command.ExecuteReader())
        //             {
        //                 while (reader.Read())
        //                 {
        //                     listeDevis.Add(new Devis(reader.GetInt32(reader.GetOrdinal("id")), 
        //                     reader.GetString(reader.GetOrdinal("numero")), ));
        //                 }
        //             }
        //         }
        //         return listeDevis;
        //     }
        //     catch (System.Exception)
        //     {

        //         throw;
        //     }
        //     finally
        //     {
        //         if (isNewConnexion == true)
        //         {
        //             connection.Close();
        //         }
        //     }
        // }

        public void Create(NpgsqlConnection connection)
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
                TypeMaison tm = new TypeMaison(this.IdTypeMaison, "", "", 0);
                tm.Find(connection);
                this.DureeDeConstruction = tm.DureeDeConstruction;
                this.Surface = tm.Surface;
                TypeFinition tf = new TypeFinition(this.IdTypeFinition, "", 0);
                tf.Find(connection);
                this.PourcentageFinition = tf.Pourcentage;
                this.Insert(connection, transaction);
                this.CreateTravauxDevis(connection, transaction);
                transaction.Commit();
            }
            catch (System.Exception)
            {
                transaction.Rollback();
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

        public void CreateTravauxDevis(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                string query = @$"INSERT INTO travauxDevis (idDevis, idTypeTravaux, quantite, prixUnitaire)
                SELECT {this.Id} AS iddevis, idTypeTravaux, quantite, prixUnitaire
                FROM travauxTypeDeMaison
                JOIN typeTravaux ON travauxTypeDeMaison.idTypeTravaux = typeTravaux.id
                WHERE idTypeMaison = {this.IdTypeMaison};";
                this.ExecuteNonQuery(connection, transaction, query);
                this.MontantTravaux = Convert.ToDouble(this.
                ExecuteQuery(connection, @$"SELECT SUM(quantite * prixUnitaire) FROM travauxDevis 
                WHERE iddevis = {this.Id}").Rows[0][0]);
                this.Update(connection, transaction, new string[] { "montantTravaux" });
            }
            catch (System.Exception)
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
    }
}