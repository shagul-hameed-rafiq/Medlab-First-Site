using MedLabAInsights.DataGenerator.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

public class ReportGenerator : IReportGenerator
{
    private readonly ITestConfigProvider _configProvider;
    private readonly IValueGenerator _valueGenerator;
    private readonly List<Test> _tests;
    public ReportGenerator(ITestConfigProvider configProvider, IValueGenerator valueGenerator)
    {
        _configProvider = configProvider;
        _valueGenerator = valueGenerator;

        _tests = _configProvider.Load()?.ToList() ?? new List<Test>();

        if (_tests.Count == 0)
        {
            Console.WriteLine("[ReportGenerator] Warning: no tests loaded from configuration.");
        }
    }
    public IEnumerable<GeneratedReport> GenerateReport(int? count)
    {
        if (count <= 0)
        {
            Console.WriteLine($"[ReportGenerator] Invalid report count: {count}. Nothing to generate.");
            yield break;
        }

        if (_tests.Count == 0)
        {
            Console.WriteLine("[ReportGenerator] Cannot generate reports because there are no tests.");
            yield break;
        }

        for (int i = 0; i < count; i++)
        {
            yield return GenerateSingleReport();
        }
    }

    private GeneratedReport GenerateSingleReport()
    {
        var dict = new Dictionary<string, GeneratedValue>(_tests.Count);

        for (int i = 0; i < _tests.Count; i++)
        {
            var test = _tests[i];

            var code = string.IsNullOrWhiteSpace(test.Code)
                ? test.Name
                : test.Code;

            var value = _valueGenerator.Generate(test);

            dict[code] = new GeneratedValue
            {
                Value = value,
                Unit = test.Unit ?? string.Empty
            };
        }

        return new GeneratedReport
        {
            Values = dict
        };
    }
    public static int GetReportSizeFromConfig()
    {
        const int defaultSize = 1;

        string? raw = ConfigurationManager.AppSettings?["ReportCount"];

        if (int.TryParse(raw, out int count) && count > 0)
            return count;

        Console.WriteLine("[ReportGenerator] Using default ReportCount = 1");
        return defaultSize;
    }
}
