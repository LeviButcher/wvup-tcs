using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using CsvHelper.Configuration;

public class CSVSessionUpload
{
    [Required]
    public string Email { get; set; }

    [Required]
    public DateTime InTime { get; set; }

    [Required]
    public DateTime OutTime { get; set; }

    [Required]
    public bool Tutoring { get; set; }
    public IEnumerable<int> CRNs { get; set; } = new List<int>();
    public IEnumerable<string> Reasons { get; set; } = new List<string>();

    [Required]
    public int SemesterCode { get; set; }
}

public sealed class CSVSessionUploadMap : ClassMap<CSVSessionUpload>
{
    public CSVSessionUploadMap()
    {
        AutoMap(CultureInfo.InvariantCulture);
        Map(m => m.CRNs).ConvertUsing(row =>
          row.GetField<string>("CRNs")
          .Split(",").Select(x => x.Trim())
          .Select(x => int.Parse(x)));
        Map(m => m.Reasons).ConvertUsing(row => row.GetField("Reasons").Split(",").Select(x => x.Trim()));
    }
}