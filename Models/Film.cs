using System.ComponentModel.DataAnnotations;

namespace Evaluation.Models
{
    public class Film : BDDObject
    {
        int id;
        string nom;
        DateTime dateDeSortie;
        int idCategorie;

        public Film()
        {
        }

        public Film(int id, string nom, DateTime dateDeSortie, int idCategorie)
        {
            Id = id;
            Nom = nom;
            DateDeSortie = dateDeSortie;
            IdCategorie = idCategorie;
        }

        public int Id { get => id; set => id = value; }
        [Required(ErrorMessage = "Le nom est obligatoire")]
        public string Nom { get => nom; set => nom = value; }
        public int IdCategorie { get => idCategorie; set => idCategorie = value; }
        public DateTime DateDeSortie { get => dateDeSortie; set => dateDeSortie = value; }

        public override string TableName()
        {
            return "film";
        }
    }
}