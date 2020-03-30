using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using System.Linq;

public class CSVSessionUploadParser : ICSVParser<CSVSessionUpload>
{
    public IEnumerable<CSVSessionUpload> Parse(IFormFile file)
    {
        var reader = new StreamReader(file.OpenReadStream());
        var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Configuration.Delimiter = ";";
        csv.Configuration.RegisterClassMap<CSVSessionUploadMap>();
        // ToList pull everything into memory, will be a probelm with large datasets
        return csv.GetRecords<CSVSessionUpload>().ToList();
    }
}