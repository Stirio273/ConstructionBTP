namespace Evaluation.Models
{
    public class TravauxTypeDeMaison : BDDObject
    {
        int id;
        int idTypeMaison;
        int idTypeTravaux;
        double quantite;
        TypeTravaux typeTravaux;

        public TravauxTypeDeMaison()
        {
        }

        public TravauxTypeDeMaison(int id, int idTypeMaison, int idTypeTravaux, double quantite, double prixUnitaire)
        {
            Id = id;
            IdTypeMaison = idTypeMaison;
            IdTypeTravaux = idTypeTravaux;
            Quantite = quantite;
        }

        public TravauxTypeDeMaison(int id, int idTypeMaison, TypeTravaux typeTravaux, double quantite,
         double prixUnitaire, double total)
        {
            Id = id;
            IdTypeMaison = idTypeMaison;
            TypeTravaux = typeTravaux;
            Quantite = quantite;
        }

        [Identity]
        public int Id { get => id; set => id = value; }
        public int IdTypeMaison { get => idTypeMaison; set => idTypeMaison = value; }
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
        public TypeTravaux TypeTravaux
        {
            get => typeTravaux;
            set
            {
                typeTravaux = value;
                idTypeTravaux = typeTravaux.Id;
            }
        }

        public override string TableName()
        {
            return "travauxtypedemaison";
        }
    }
}