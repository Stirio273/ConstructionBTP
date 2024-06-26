using Npgsql;
namespace Evaluation.Models
{
    public class StdTemp : BDDObject
    {
        int row;
        string error;

        public StdTemp()
        {
        }

        public int Row { get => row; set => row = value; }
        public string Error { get => error; set => error = value; }

        public override string TableName()
        {
            return "std_temp";
        }

        public virtual void Validate(NpgsqlConnection connection)
        {
            return;
        }
    }
}