namespace Evaluation.Models
{
    public class Categorie : BDDObject
    {
        int id;
        string nom;

        public Categorie()
        {
        }

        public Categorie(int id, string nom)
        {
            Id = id;
            Nom = nom;
        }

        public int Id { get => id; set => id = value; }
        public string Nom { get => nom; set => nom = value; }

        public override string TableName()
        {
            return "categorie";
        }
    }
}