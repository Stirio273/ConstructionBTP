namespace Evaluation.Models
{
    public class Unite : BDDObject
    {
        int id;
        string nom;

        public Unite()
        {
        }

        public Unite(int id, string nom)
        {
            Id = id;
            Nom = nom;
        }

        [Default]
        public int Id { get => id; set => id = value; }
        public string Nom { get => nom; set => nom = value; }

        public override string TableName()
        {
            return "unite";
        }
    }
}