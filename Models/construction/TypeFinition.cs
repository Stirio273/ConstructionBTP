namespace Evaluation.Models
{
    public class TypeFinition : BDDObject
    {
        int id;
        string nom;
        double pourcentage;

        public TypeFinition()
        {
        }

        public TypeFinition(int id, string nom, double pourcentage)
        {
            Id = id;
            Nom = nom;
            Pourcentage = pourcentage;
        }

        [Identity]
        public int Id { get => id; set => id = value; }
        public string Nom { get => nom; set => nom = value; }
        public double Pourcentage { get => pourcentage; set => pourcentage = value; }

        public override string TableName()
        {
            return "typefinition";
        }
    }
}