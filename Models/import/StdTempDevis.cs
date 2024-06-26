using Npgsql;

namespace Evaluation.Models
{
    public class StdTempDevis : StdTemp
    {
        string client;
        string ref_devis;
        string type_maison;
        string finition;
        string taux_finition;
        string date_devis;
        string date_debut;
        string lieu;


        public StdTempDevis()
        {
        }

        public StdTempDevis(int row)
        {
            Row = row;
        }
        public string Client { get => client; set => client = value; }
        public string Ref_devis { get => ref_devis; set => ref_devis = value; }
        public string Type_maison { get => type_maison; set => type_maison = value; }
        public string Finition { get => finition; set => finition = value; }
        public string Taux_finition { get => taux_finition; set => taux_finition = value; }
        public string Date_devis { get => date_devis; set => date_devis = value; }
        public string Date_debut { get => date_debut; set => date_debut = value; }
        public string Lieu { get => lieu; set => lieu = value; }

        public override void Validate(NpgsqlConnection connection)
        {
            List<string> errors = new List<string>();
            string errorClient = ValidateClient();
            if (!string.IsNullOrEmpty(errorClient))
            {
                errors.Add(errorClient);
            }
            string errorRefDevis = ValidateRefDevis();
            if (!string.IsNullOrEmpty(errorRefDevis))
            {
                errors.Add(errorRefDevis);
            }
            string errorTypeMaison = ValidateTypeMaison();
            if (!string.IsNullOrEmpty(errorTypeMaison))
            {
                errors.Add(errorTypeMaison);
            }
            string errorFinition = ValidateFinition();
            if (!string.IsNullOrEmpty(errorFinition))
            {
                errors.Add(errorFinition);
            }
            string errorTauxFinition = ValidateTauxFinition();
            if (!string.IsNullOrEmpty(errorTauxFinition))
            {
                errors.Add(errorTauxFinition);
            }
            string errorDateDevis = ValidateDateDevis();
            if (!string.IsNullOrEmpty(errorDateDevis))
            {
                errors.Add(errorDateDevis);
            }
            string errorDateDebut = ValidateDateDebut();
            if (!string.IsNullOrEmpty(errorDateDebut))
            {
                errors.Add(errorDateDebut);
            }
            string errorLieu = ValidateLieu();
            if (!string.IsNullOrEmpty(errorLieu))
            {
                errors.Add(errorLieu);
            }
            if (errors.Count > 0)
            {
                Error = string.Join(", ", errors) + $": a la ligne {Row}";
            }
            return;
        }

        private string ValidateClient()
        {
            if (false)
            {
                return "Error message for property1";
            }

            return string.Empty;
        }

        private string ValidateRefDevis()
        {
            if (false)
            {
                return "Error message for property1";
            }

            return string.Empty;
        }

        private string ValidateTypeMaison()
        {
            if (false)
            {
                return "Error message for property1";
            }

            return string.Empty;
        }

        private string ValidateFinition()
        {
            if (false)
            {
                return "Error message for property1";
            }

            return string.Empty;
        }

        private string ValidateTauxFinition()
        {
            if (false)
            {
                return "Error message for property1";
            }

            return string.Empty;
        }

        private string ValidateDateDevis()
        {
            if (false)
            {
                return "Error message for property1";
            }

            return string.Empty;
        }

        private string ValidateDateDebut()
        {
            if (false)
            {
                return "Error message for property1";
            }

            return string.Empty;
        }

        private string ValidateLieu()
        {
            if (false)
            {
                return "Error message for property1";
            }

            return string.Empty;
        }
    }
}