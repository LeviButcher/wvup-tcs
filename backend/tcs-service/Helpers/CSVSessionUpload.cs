using System;
using System.Collections.Generic;
using CsvHelper.Configuration;
using System.Linq;
using System.Globalization;

public class CSVSessionUpload
{
    public string Email { get; set; }
    public DateTime InTime { get; set; }
    public DateTime OutTime { get; set; }
    public bool Tutoring { get; set; }
    public IEnumerable<int> CRNs { get; set; }
    public IEnumerable<string> Reasons { get; set; }
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