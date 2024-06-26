using Npgsql;

namespace Evaluation.Models
{
    public class TypeMaison : BDDObject
    {
        int id;
        string nom;
        string description;
        int dureeDeConstruction;
        double surface;

        public TypeMaison()
        {
        }

        public TypeMaison(int id, string nom, string description, int dureeDeConstruction)
        {
            Id = id;
            Nom = nom;
            Description = description;
            DureeDeConstruction = dureeDeConstruction;
        }

        [Identity]
        public int Id { get => id; set => id = value; }
        public string Nom { get => nom; set => nom = value; }
        public string Description { get => description; set => description = value; }
        public int DureeDeConstruction { get => dureeDeConstruction; set => dureeDeConstruction = value; }
        public double Surface { get => surface; set => surface = value; }

        public override string TableName()
        {
            return "typemaison";
        }

        public List<TravauxTypeDeMaison> GetAllTravaux(NpgsqlConnection connection, string filtre)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                List<TravauxTypeDeMaison> listeTravaux = new List<TravauxTypeDeMaison>();
                string query = $"SELECT * FROM vw_travauxTypeDeMaison WHERE idTypeMaison = @idTypeMaison {filtre}";
                using (var command = connection.CreateCommand())
                {
                    command.Parameters.AddWithValue("idTypeMaison", this.Id);
                    command.CommandText = query;
                    command.Connection = connection;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listeTravaux.Add(new TravauxTypeDeMaison(reader.GetInt32(reader.GetOrdinal("id")),
                            this.Id, new TypeTravaux(reader.GetInt32(reader.GetOrdinal("idTypeTravaux")),
                            reader.GetString(reader.GetOrdinal("numero")),
                            reader.GetString(reader.GetOrdinal("designation")),
                            new Unite(0, reader.GetString(reader.GetOrdinal("unite")))),
                            reader.GetDouble(reader.GetOrdinal("prixunitaire")),
                            reader.GetDouble(reader.GetOrdinal("quantite")), reader.GetDouble(reader.GetOrdinal("total"))));
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
    }
}