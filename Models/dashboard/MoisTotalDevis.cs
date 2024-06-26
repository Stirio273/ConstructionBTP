namespace Evaluation.Models
{
    public class MoisTotalDevis : BDDObject
    {
        int mois;
        double montantTotalDevis;
        int annee;

        public MoisTotalDevis()
        {
        }

        public MoisTotalDevis(int annee)
        {
            Annee = annee;
        }

        public MoisTotalDevis(int mois, double montantTotalDevis, int annee)
        {
            Mois = mois;
            MontantTotalDevis = montantTotalDevis;
            Annee = annee;
        }

        public int Mois { get => mois; set => mois = value; }
        public double MontantTotalDevis { get => montantTotalDevis; set => montantTotalDevis = value; }
        public int Annee { get => annee; set => annee = value; }

        public override string TableName()
        {
            return $"total_devis_par_mois({Annee})";
        }
    }
}