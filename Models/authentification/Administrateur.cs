using System;
using System.Collections.Generic;
using Npgsql;

namespace Evaluation.Models;

public class Administrateur : BDDObject
{
    private int id;
    private string email;
    private string password;
    private int idProfil;

    public Administrateur()
    {
    }

    public Administrateur(string? email, string? password)
    {
        Email = email;
        Password = password;
    }

    public Administrateur(int id, string? email, string? password, int idProfil)
    {
        Id = id;
        Email = email;
        Password = password;
        IdProfil = idProfil;
    }

    [Identity]
    public int Id { get => id; set => id = value; }
    public string Email { get => email; set => email = value; }
    public string Password { get => password; set => password = value; }
    public int IdProfil { get => idProfil; set => idProfil = value; }

    public override string TableName()
    {
        return "administrateurs";
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
            string query = $"SELECT * FROM vw_devis_paiementEffectue {filtre}";
            Console.WriteLine(query);
            using (var command = connection.CreateCommand())
            {
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
                         reader.GetString(reader.GetOrdinal("nomMaison")), reader.GetDouble(reader.GetOrdinal("pourcentageFinition"))),
                         reader.GetDateTime(reader.GetOrdinal("dateDebut")), reader.GetDouble(reader.GetOrdinal("montantTravaux")),
                         reader.GetDouble(reader.GetOrdinal("paiementEffectue"))));
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

    public Administrateur GetAdministrateurByEmail(NpgsqlConnection connection)
    {
        bool isNewConnexion = false;
        if (connection == null)
        {
            connection = new Connect().getConnectPostgres();
            isNewConnexion = true;
        }
        try
        {
            string query = "SELECT * FROM administrateurs WHERE email = @email";
            Console.WriteLine(query);
            using (var command = connection.CreateCommand())//new NpgsqlCommand(query, con))
            {
                command.Parameters.AddWithValue("email", this.Email);
                command.CommandText = query;
                command.Connection = connection;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return new Administrateur(reader.GetInt16(reader.GetOrdinal("id")),
                        reader.GetString(reader.GetOrdinal("email")),
                        reader.GetString(reader.GetOrdinal("password")),
                        reader.GetInt16(reader.GetOrdinal("idProfil")));
                    }
                }
            }
            throw new Exception("Invalid email or password");
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
