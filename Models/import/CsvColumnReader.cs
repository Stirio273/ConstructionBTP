using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

public class CsvColumnReader
{
    public IEnumerable<string> GetColumnNames(string filePath)
    {
        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read();
            csv.ReadHeader();
            return csv.HeaderRecord[0].Split(";");
        }
    }
}
