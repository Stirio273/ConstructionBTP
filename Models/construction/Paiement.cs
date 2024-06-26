using Npgsql;

namespace Evaluation.Models
{
    public class Paiement : BDDObject
    {
        int id;
        int idDevis;
        string ref_paiement;
        DateTime date;
        double montant;

        public Paiement()
        {
        }

        public Paiement(int id, int idDevis, DateTime date, double montant)
        {
            Id = id;
            IdDevis = idDevis;
            Date = date;
            Montant = montant;
        }

        [Identity]
        public int Id { get => id; set => id = value; }
        public int IdDevis { get => idDevis; set => idDevis = value; }
        public DateTime Date { get => date; set => date = value; }
        public double Montant { get => montant; set => montant = value; }
        [Default]
        public string Ref_paiement { get => ref_paiement; set => ref_paiement = value; }

        public override string TableName()
        {
            return "paiement";
        }

        public double GetMontantPayer(NpgsqlConnection connection, Devis devis)
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
                @$"SELECT getMontantPayer({devis.Id})").Rows[0][0]);
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