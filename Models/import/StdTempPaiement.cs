using Npgsql;

namespace Evaluation.Models
{
    public class StdTempPaiement : StdTemp
    {
        string ref_devis;
        string ref_paiement;
        string date_paiement;
        string montant;

        public StdTempPaiement()
        {
        }

        public StdTempPaiement(int row)
        {
            Row = row;
        }
        public string Ref_devis { get => ref_devis; set => ref_devis = value; }
        public string Ref_paiement { get => ref_paiement; set => ref_paiement = value; }
        public string Date_paiement { get => date_paiement; set => date_paiement = value; }
        public string Montant { get => montant; set => montant = value; }

        public override void Validate(NpgsqlConnection connection)
        {
            List<string> errors = new List<string>();
            string errorRefDevis = ValidateRefDevis();
            if (!string.IsNullOrEmpty(errorRefDevis))
            {
                errors.Add(errorRefDevis);
            }
            string errorRefPaiement = ValidateRefPaiement(connection);
            if (!string.IsNullOrEmpty(errorRefPaiement))
            {
                errors.Add(errorRefPaiement);
            }
            string errorDatePaiement = ValidateDatePaiement();
            if (!string.IsNullOrEmpty(errorDatePaiement))
            {
                errors.Add(errorDatePaiement);
            }
            string errorMontant = ValidateMontant();
            if (!string.IsNullOrEmpty(errorMontant))
            {
                errors.Add(errorMontant);
            }
            if (errors.Count > 0)
            {
                Error = string.Join(", ", errors) + $": a la ligne {Row}";
            }
            return;
        }

        private string ValidateRefDevis()
        {
            if (false)
            {
                return "Error message for property1";
            }

            return string.Empty;
        }

        private string ValidateRefPaiement(NpgsqlConnection connection)
        {
            List<Paiement> paiements = new Paiement().Find<Paiement>(connection, $"WHERE ref_paiement = '{Ref_paiement}'");
            List<StdTempPaiement> tempPaiements = new StdTempPaiement().Find<StdTempPaiement>(connection, $"WHERE ref_paiement = '{Ref_paiement}'");
            if (tempPaiements.Count > 1)
            {
                for (int i = 1; i < tempPaiements.Count; i++)
                {
                    tempPaiements[i].ExecuteNonQuery(connection, null, @$"DELETE FROM std_temp WHERE 
                        row = {tempPaiements[i].Row}");
                }
            }
            if (paiements.Count > 0)
            {
                return "Ce reference paiement existe deja";
            }

            return string.Empty;

        }

        private string ValidateDatePaiement()
        {
            // Complex validation logic for property1...
            if (false)
            {
                return "Error message for property1";
            }

            return string.Empty;
        }

        private string ValidateMontant()
        {

            if (Double.Parse(montant) < 0)
            {
                return "Valeur negatif pour le montant";
            }

            return string.Empty;
        }
    }
}