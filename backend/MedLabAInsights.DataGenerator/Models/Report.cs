namespace MedLabAInsights.DataGenerator.Models
{
    public class GeneratedReport
    {
        public required Dictionary<string, GeneratedValue> Values { get; set; }
    }
    public class GeneratedValue
    {
        public required object? Value { get; set; }
        public required string? Unit { get; set; }
    }
}
