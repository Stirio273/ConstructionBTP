namespace Evaluation.Models
{
    public class ProfilList
    {
        private static List<Profil> profils;

        static ProfilList()
        {
            Profils = new Profil().Find<Profil>(null, null);
        }

        public static List<Profil> Profils { get => profils; set => profils = value; }

        public static string GetProfilName(int id)
        {
            List<Profil> lp = Profils.Where(p => p.Id == id).ToList();
            if (lp.Count > 0)
            {
                return lp[0].Nom;
            }
            return "Anonymous";
        }
    }
}