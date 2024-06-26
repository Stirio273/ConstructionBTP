using Npgsql;

namespace Evaluation.Models
{
    public class StdTempMaisonTravaux : StdTemp
    {
        string type_maison;
        string description;
        string surface;
        string code_travaux;
        string type_travaux;
        string unite;
        string prix_unitaire;
        string quantite;
        string duree_travaux;

        public StdTempMaisonTravaux()
        {
        }

        public StdTempMaisonTravaux(int row)
        {
            Row = row;
        }

        public string Type_maison { get => type_maison; set => type_maison = value; }
        public string Description { get => description; set => description = value; }
        public string Surface { get => surface; set => surface = value; }
        public string Code_travaux { get => code_travaux; set => code_travaux = value; }
        public string Type_travaux { get => type_travaux; set => type_travaux = value; }
        public string Unite { get => unite; set => unite = value; }
        public string Prix_unitaire { get => prix_unitaire; set => prix_unitaire = value; }
        public string Quantite { get => quantite; set => quantite = value; }
        public string Duree_travaux { get => duree_travaux; set => duree_travaux = value; }


        public override void Validate(NpgsqlConnection connection)
        {
            List<string> errors = new List<string>();
            string errorTypeMaison = ValidateTypeMaison();
            if (!string.IsNullOrEmpty(errorTypeMaison))
            {
                errors.Add(errorTypeMaison);
            }
            string errorDescription = ValidateDescription();
            if (!string.IsNullOrEmpty(errorDescription))
            {
                errors.Add(errorDescription);
            }
            string errorSurface = ValidateSurface();
            if (!string.IsNullOrEmpty(errorSurface))
            {
                errors.Add(errorSurface);
            }
            string errorCodeTravaux = ValidateCodeTravaux();
            if (!string.IsNullOrEmpty(errorCodeTravaux))
            {
                errors.Add(errorCodeTravaux);
            }
            string errorTypeTravaux = ValidateTypeTravaux();
            if (!string.IsNullOrEmpty(errorTypeTravaux))
            {
                errors.Add(errorTypeTravaux);
            }
            string errorUnite = ValidateUnite();
            if (!string.IsNullOrEmpty(errorUnite))
            {
                errors.Add(errorUnite);
            }
            string errorPrixUnitaire = ValidatePrixUnitaire();
            if (!string.IsNullOrEmpty(errorPrixUnitaire))
            {
                errors.Add(errorPrixUnitaire);
            }
            string errorQuantite = ValidateQuantite();
            if (!string.IsNullOrEmpty(errorQuantite))
            {
                errors.Add(errorQuantite);
            }
            string errorDureeTravaux = ValidateDureeTravaux();
            if (!string.IsNullOrEmpty(errorDureeTravaux))
            {
                errors.Add(errorDureeTravaux);
            }
            if (errors.Count > 0)
            {
                Error = string.Join(", ", errors) + $": a la ligne {Row}";
            }
            return;
        }

        private string ValidateTypeMaison()
        {
            if (false)
            {
                return "Error message for property1";
            }

            return string.Empty;
        }

        private string ValidateDescription()
        {
            if (false)
            {
                return "Error message for property1";
            }

            return string.Empty;
        }

        private string ValidateSurface()
        {
            if (false)
            {
                return "Error message for property1";
            }

            return string.Empty;
        }

        private string ValidateCodeTravaux()
        {
            if (false)
            {
                return "Error message for property1";
            }

            return string.Empty;
        }

        private string ValidateTypeTravaux()
        {
            if (false)
            {
                return "Error message for property1";
            }

            return string.Empty;
        }

        private string ValidateUnite()
        {
            if (false)
            {
                return "Error message for property1";
            }

            return string.Empty;
        }

        private string ValidatePrixUnitaire()
        {
            if (false)
            {
                return "Error message for property1";
            }

            return string.Empty;
        }

        private string ValidateQuantite()
        {
            if (false)
            {
                return "Error message for property1";
            }

            return string.Empty;
        }

        private string ValidateDureeTravaux()
        {
            if (false)
            {
                return "Error message for property1";
            }

            return string.Empty;
        }
    }
}