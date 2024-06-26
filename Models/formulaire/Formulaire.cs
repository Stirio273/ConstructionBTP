using Npgsql;

namespace Evaluation.Models
{
    public class Formulaire
    {
        public static void GetDataForCreateDevisUser(ref List<TypeMaison> typeMaisons, ref List<TypeFinition> finitions)
        {
            NpgsqlConnection connection = new Connect().getConnectPostgres();
            try
            {
                typeMaisons = new TypeMaison().Find<TypeMaison>(connection, null);
                finitions = new TypeFinition().Find<TypeFinition>(connection, null);
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}