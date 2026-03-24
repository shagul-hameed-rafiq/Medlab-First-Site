using MedLabAInsights.DataGenerator.Models;

public class ValueGenerator : IValueGenerator
{
    private readonly Random _rand = new Random();

    public object? Generate(Test test)
    {
        if (test == null)
        {
            Console.WriteLine("[ValueGenerator] Test object is null.");
            return null;
        }

        var type = test.DataType?.ToLower();

        return type switch
        {
            "float" => GenerateFloat(test),
            "int" => GenerateInt(test),
            "categorical" => GenerateCategory(test),
            _ => HandleUnknownType(test)
        };
    }

    private object? GenerateFloat(Test test)
    {
        if (test.Meta?.Range == null)
        {
            Console.WriteLine($"[ValueGenerator] Missing range for float test '{test.Name}'.");
            return null;
        }

        double min = test.Meta.Range.Min;
        double max = test.Meta.Range.Max;
        int precision = test.Meta.Precision ?? 2;

        double val = min + _rand.NextDouble() * (max - min);
        return Math.Round(val, precision);
    }

    private object? GenerateInt(Test test)
    {
        if (test.Meta?.Range == null)
        {
            Console.WriteLine($"[ValueGenerator] Missing range for int test '{test.Name}'.");
            return null;
        }

        int min = (int)test.Meta.Range.Min;
        int max = (int)test.Meta.Range.Max;

        return _rand.Next(min, max + 1);
    }

    private object? GenerateCategory(Test test)
    {
        var values = test.Meta?.CustomValues;

        if (values == null || values.Count == 0)
        {
            Console.WriteLine($"[ValueGenerator] No categorical choices provided for '{test.Name}'.");
            return null;
        }

        int index = _rand.Next(values.Count);
        return values[index];
    }

    private static object? HandleUnknownType(Test test)
    {
        Console.WriteLine($"[ValueGenerator] Unknown dataType '{test.DataType}' for test '{test.Name}'.");
        return null;
    }
}
