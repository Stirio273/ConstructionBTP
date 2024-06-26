using Npgsql;

namespace Evaluation.Models
{
    public class Dashboard
    {
        double montantTotalDevis;
        List<MoisTotalDevis> moisTotalDevis;
        double montantTotalPaiementEffectue;

        public double MontantTotalDevis { get => montantTotalDevis; set => montantTotalDevis = value; }
        public List<MoisTotalDevis> MoisTotalDevis { get => moisTotalDevis; set => moisTotalDevis = value; }
        public double MontantTotalPaiementEffectue { get => montantTotalPaiementEffectue; set => montantTotalPaiementEffectue = value; }

        public void Initialize()
        {
            NpgsqlConnection connection = new Connect().getConnectPostgres();
            try
            {
                MontantTotalDevis = GetMontantTotalDevis(connection);
                MontantTotalPaiementEffectue = GetMontantTotalPaiementEffectue(connection);
                // MoisTotalDevis = GetMontantTotalDevisParMois(connection, DateTime.Now.Year);
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public static double GetMontantTotalPaiementEffectue(NpgsqlConnection connection)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                string query = @"SELECT COALESCE(SUM(montant), 0) AS montantTotalPaiementEffectue
                FROM paiement;";
                return Convert.ToDouble(new BDDObject().ExecuteQuery(connection, query).Rows[0][0]);
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

        public static double GetMontantTotalDevis(NpgsqlConnection connection)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                string query = @"SELECT COALESCE(SUM(montantTravaux * (1 + (pourcentageFinition / 100))), 0) 
                AS montantTotalDevis FROM devis;";
                return Convert.ToDouble(new BDDObject().ExecuteQuery(connection, query).Rows[0][0]);
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

        public static List<MoisTotalDevis> GetMontantTotalDevisParMois(NpgsqlConnection connection, int annee)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                return new MoisTotalDevis(annee).Find<MoisTotalDevis>(connection, "");
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