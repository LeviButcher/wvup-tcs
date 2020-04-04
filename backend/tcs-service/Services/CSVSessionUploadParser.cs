using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using tcs_service.Helpers;

namespace tcs_service.Services {
    ///<summary>Parses a CSV File into a List of a CSVSessionUpload</summary>
    public class CSVSessionUploadParser : ICSVParser<CSVSessionUpload> {
        ///<summary>Parses a CSV File into a List of a CSVSessionUpload</summary>
        /// CSV File must be delimited with ';' and Reasons and CRNs must be comma separated
        public IEnumerable<CSVSessionUpload> Parse (IFormFile file) {
            var reader = new StreamReader (file.OpenReadStream ());
            var csv = new CsvReader (reader, CultureInfo.InvariantCulture);
            csv.Configuration.Delimiter = ";";
            csv.Configuration.RegisterClassMap<CSVSessionUploadMap> ();
            // ToList pull everything into memory, will be a problem with large datasets
            return csv.GetRecords<CSVSessionUpload> ().ToList ();
        }
    }
}