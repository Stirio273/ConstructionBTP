namespace Evaluation.Models
{
    public class Profil : BDDObject
    {
        private int id;
        private string nom;

        public Profil()
        {
        }

        public Profil(int id, string nom)
        {
            Id = id;
            Nom = nom;
        }

        [Identity]
        public int Id { get => id; set => id = value; }
        public string Nom { get => nom; set => nom = value; }

        public override string TableName()
        {
            return "profil";
        }
    }
}