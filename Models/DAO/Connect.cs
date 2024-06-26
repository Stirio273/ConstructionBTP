using Npgsql;

namespace Evaluation.Models
{
    internal class Connect
    {
        public NpgsqlConnection getConnectPostgres()
        {
            try
            {
                var configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
                string connString = configuration.GetConnectionString("Default");
                Console.Write("Connecting to PostgreSQL Server ... ");
                NpgsqlConnection connection = new NpgsqlConnection(connString);
                Console.WriteLine(connection);
                connection.Open();
                Console.WriteLine("Done.");
                return connection;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
