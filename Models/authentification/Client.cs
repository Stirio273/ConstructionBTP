using Npgsql;
using System.ComponentModel.DataAnnotations;

namespace Evaluation.Models
{
    public class Client : BDDObject
    {
        int id;
        string numero;
        int idProfil;

        public Client() { }

        public Client(string numero)
        {
            Numero = numero;
        }

        public Client(int id, string numero, int idProfil)
        {
            Id = id;
            Numero = numero;
            IdProfil = idProfil;
        }

        [Identity]
        public int Id { get => id; set => id = value; }
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Format non valide")]
        public string Numero { get => numero; set => numero = value; }
        public int IdProfil { get => idProfil; set => idProfil = value; }

        public override string TableName()
        {
            return "clients";
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
                string query = $"SELECT * FROM vw_devis WHERE idClient = @idClient {filtre}";
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

        public Client GetClientByPhoneNumber(NpgsqlConnection connection)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                string query = "SELECT * FROM clients WHERE numero = @numero";
                Console.WriteLine(query);
                using (var command = connection.CreateCommand())//new NpgsqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("numero", this.Numero);
                    command.CommandText = query;
                    command.Connection = connection;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return new Client(reader.GetInt16(reader.GetOrdinal("id")),
                            reader.GetString(reader.GetOrdinal("numero")),
                            reader.GetInt16(reader.GetOrdinal("idProfil")));
                        }
                    }
                }
                this.IdProfil = 2;
                this.Insert(connection, null);
                //this.Id = Convert.ToInt32(this.ExecuteQuery(connection, "SELECT Max(id) FROM clients;").Rows[0][0]);
                return this;
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
    }
}