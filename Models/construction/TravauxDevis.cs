namespace Evaluation.Models
{
    public class TravauxDevis : BDDObject
    {
        int id;
        int idDevis;
        int idTypeTravaux;
        double quantite;
        double prixUnitaire;
        double total;
        Devis devis;
        TypeTravaux typeTravaux;

        public TravauxDevis()
        {
        }

        public TravauxDevis(int id, int idDevis, double quantite, double prixUnitaire,
        TypeTravaux typeTravaux, double total)
        {
            Id = id;
            IdDevis = idDevis;
            Quantite = quantite;
            PrixUnitaire = prixUnitaire;
            TypeTravaux = typeTravaux;
            Total = total;
        }

        [Identity]
        public int Id { get => id; set => id = value; }
        public int IdTypeTravaux
        {
            get => idTypeTravaux;
            set
            {
                idTypeTravaux = value;
                if (TypeTravaux == null)
                {
                    TypeTravaux = new TypeTravaux(idTypeTravaux, "", "", 0);
                    return;
                }
                TypeTravaux.Id = idTypeTravaux;
            }
        }
        public double Quantite { get => quantite; set => quantite = value; }
        public double PrixUnitaire { get => PrixUnitaire1; set => PrixUnitaire1 = value; }
        public TypeTravaux TypeTravaux
        {
            get => typeTravaux;
            set
            {
                typeTravaux = value;
                idTypeTravaux = typeTravaux.Id;
            }
        }

        public int IdDevis
        {
            get => idDevis;
            set
            {
                idDevis = value;
                if (Devis == null)
                {
                    Devis = new Devis();
                }
                Devis.Id = idDevis;
            }
        }
        public Devis Devis
        {
            get => devis;
            set
            {
                devis = value;
                idDevis = devis.Id;
            }
        }

        public double PrixUnitaire1 { get => prixUnitaire; set => prixUnitaire = value; }
        public double Total { get => total; set => total = value; }

        public override string TableName()
        {
            return "travauxdevis";
        }
    }
}