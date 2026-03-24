using MedLabAInsights.DataGenerator.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;

class Program
{
    static void Main()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);

        using var provider = services.BuildServiceProvider();

        var reportGenerator = provider.GetRequiredService<IReportGenerator>();
        var exporter = provider.GetRequiredService<IReportExporter>();

        int size = ReportGenerator.GetReportSizeFromConfig();
        var report = reportGenerator.GenerateReport(size);

        string? exportPath = ConfigurationManager.AppSettings?["ExportPath"];
        if (string.IsNullOrWhiteSpace(exportPath))
        {
            exportPath = "Reports.csv";
        }
        exporter.Export(report);

        Console.WriteLine("Done.");
        Console.ReadLine();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ITestConfigProvider>(sp =>
        {
            string? path = ConfigurationManager.AppSettings?["TestConfigPath"];
            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine("[Program] No TestConfigPath found");
            }
            return new TestConfigProvider(path);
        });
        services.AddSingleton<IReportExporter>(sp =>
        {
            string? path = ConfigurationManager.AppSettings?["ExportPath"];
            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine("[Program] No ExportPath found");
            }
            return new ReportExporter(path);
        });


        services.AddSingleton<IValueGenerator, ValueGenerator>();
        services.AddSingleton<IReportGenerator, ReportGenerator>();
    }
}
