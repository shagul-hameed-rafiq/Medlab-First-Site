using MedLabAInsights.DataGenerator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ReportExporter : IReportExporter
{
    private readonly string? _exportPath;
    public ReportExporter(string? exportPath)
    {
        _exportPath = exportPath;
    }
    public void Export(IEnumerable<GeneratedReport> report)
    {
        if (report == null || _exportPath == null)
        {
            Console.WriteLine("[CsvReportExporter] No reports provided or no export path configured.");
            return;
        }

        var directory = Path.GetDirectoryName(_exportPath);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var writer = new StreamWriter(_exportPath);

        using var enumerator = report.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            Console.WriteLine("[CsvReportExporter] No reports to export.");
            return;
        }

        var firstReport = enumerator.Current;
        var codes = firstReport.Values.Keys.ToList();

        writer.WriteLine(string.Join(",", codes.Select(Escape)));

        WriteRow(writer, firstReport, codes);

        while (enumerator.MoveNext())
        {
            WriteRow(writer, enumerator.Current, codes);
        }

        Console.WriteLine($"[CsvReportExporter] Export completed to: {_exportPath}");
    }

    private static void WriteRow(StreamWriter writer, GeneratedReport report, List<string> codes)
    {
        var cells = new List<string>(codes.Count);

        foreach (var code in codes)
        {
            if (report.Values.TryGetValue(code, out var gv) && gv.Value != null)
            {
                cells.Add(Escape(gv.Value.ToString()));
            }
            else
            {
                cells.Add(string.Empty);
            }
        }

        writer.WriteLine(string.Join(",", cells));
    }

    private static string Escape(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        bool mustQuote = value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r');

        if (!mustQuote)
            return value;

        var escaped = value.Replace("\"", "\"\"");
        return $"\"{escaped}\"";
    }
}
