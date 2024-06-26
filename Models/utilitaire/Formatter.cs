using System.Globalization;

namespace Evaluation.Models
{
    public class Formatter
    {
        public static string FormatDouble(double d)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberGroupSeparator = " ";
            return d.ToString("N2", nfi);
        }
    }
}