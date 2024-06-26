using Npgsql;

namespace Evaluation.Models
{
    public class TypeTravaux : BDDObject
    {
        int id;
        string numero;
        string designation;
        int idUnite;
        double prixUnitaire;
        Unite unite;

        public TypeTravaux()
        {
        }

        public TypeTravaux(int id, string numero, string designation, int idUnite)
        {
            Id = id;
            Numero = numero;
            Designation = designation;
            IdUnite = idUnite;
        }

        public TypeTravaux(int id, string numero, string designation, Unite unite)
        {
            Id = id;
            Numero = numero;
            Designation = designation;
            Unite = unite;
        }
        public TypeTravaux(int id, string numero, string designation, Unite unite, double prixUnitaire)
        {
            Id = id;
            Numero = numero;
            Designation = designation;
            Unite = unite;
            PrixUnitaire = prixUnitaire;
        }

        [Identity]
        public int Id { get => id; set => id = value; }
        public string Numero { get => numero; set => numero = value; }
        public string Designation { get => designation; set => designation = value; }
        public int IdUnite
        {
            get => idUnite;
            set
            {
                idUnite = value;
                if (Unite == null)
                {
                    Unite = new Unite(idUnite, "");
                    return;
                }
                Unite.Id = idUnite;
            }
        }
        public Unite Unite
        {
            get => unite;
            set
            {
                unite = value;
                idUnite = unite.Id;
            }
        }

        public double PrixUnitaire { get => prixUnitaire; set => prixUnitaire = value; }

        public string ShowPrixUnitaire()
        {
            return Formatter.FormatDouble(PrixUnitaire);
        }

        public List<TypeTravaux> GetAllTypeTravaux(NpgsqlConnection connection, string filtre)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                List<TypeTravaux> listeTravaux = new List<TypeTravaux>();
                string query = $"SELECT * FROM vw_typeTravaux {filtre}";
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Connection = connection;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listeTravaux.Add(new TypeTravaux(reader.GetInt32(reader.GetOrdinal("id")),
                            reader.GetString(reader.GetOrdinal("numero")),
                            reader.GetString(reader.GetOrdinal("designation")), new
                            Unite(reader.GetInt32(reader.GetOrdinal("idunite")),
                            reader.GetString(reader.GetOrdinal("nomunite"))),
                            reader.GetDouble(reader.GetOrdinal("prixunitaire"))));
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

        public override string TableName()
        {
            return "typetravaux";
        }
    }
}