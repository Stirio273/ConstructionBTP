using System.Data.Common;
using Microsoft.AspNetCore.Identity;
using Npgsql;

namespace Evaluation.Models
{
    public class Reset : BDDObject
    {
        public void ResetDatabase()
        {
            NpgsqlConnection connection = new Connect().getConnectPostgres();
            NpgsqlTransaction transaction = connection.BeginTransaction();
            try
            {
                string query = @"SELECT resetDatabase();";
                this.ExecuteNonQuery(connection, null, query);
                var hasher = new PasswordHasher<string>(null);
                new Administrateur(0, "admin@gmail.com", hasher.HashPassword(null, "adminmdp"), 1).
                Insert(connection, transaction);
                transaction.Commit();
            }
            catch (System.Exception)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}