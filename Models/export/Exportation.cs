using System.Data.Common;

namespace Evaluation.Models
{
    public class Exportation
    {
        public void ExportCSV(DbConnection connection)
        {
            bool isNewConnexion = false;
            // Create a new connection if the provided connection is null
            if (connection == null)
            {
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                string query = "COPY (SELECT film.nom, categorie.nom AS categorie FROM film " +
                "JOIN categorie ON categorie.id = film.idCategorie) TO " +
                "'C:\\Users\\ASUS\\Downloads\\films.csv' DELIMITER ';' CSV HEADER";
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                // Close the connection if it was created within this method
                if (isNewConnexion == true)
                {
                    connection.Close();
                }
            }
        }

        public void ExportExcel()
        {

        }
    }
}