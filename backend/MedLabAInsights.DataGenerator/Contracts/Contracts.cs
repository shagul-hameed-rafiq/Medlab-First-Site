namespace MedLabAInsights.DataGenerator.Models
{
    public interface ITestConfigProvider
    {
        IEnumerable<Test> Load();
    }
    public interface IReportExporter
    {
        void Export(IEnumerable<GeneratedReport> report);
    }
    public interface IValueGenerator
    {
        object? Generate(Test test);
    }

    public interface IRuleEngine
    {
        void ApplyIndividualRules(GeneratedReport report);
        void ApplyCascadingRules(GeneratedReport report);
    }

    public interface IReportGenerator
    {
        IEnumerable<GeneratedReport> GenerateReport(int? size);
    }
}
