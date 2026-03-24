using MedLabAInsights.DataGenerator.Models;
using System.Text.Json;

public class TestConfigProvider : ITestConfigProvider
{
    private readonly string? _sourcePath;

    public TestConfigProvider(string? sourcePath)
    {
        _sourcePath = sourcePath;
    }

    public IEnumerable<Test> Load()
    {
        if (!File.Exists(_sourcePath))
        {
            Console.WriteLine($"[TestConfigProvider] Config file not found: {_sourcePath}");
            return [];
        }

        try
        {
            var json = File.ReadAllText(_sourcePath);

            var tests = JsonSerializer.Deserialize<List<Test>>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            if (tests == null)
            {
                Console.WriteLine("[TestConfigProvider] JSON deserialization returned null.");
                return [];
            }
            return tests;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TestConfigProvider] Error reading config: {ex.Message}");
            return [];
        }
    }
}
