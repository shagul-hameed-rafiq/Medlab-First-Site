using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace MedLabAInsights.Data.Infrastructure
{
    public static class CsvDataLoader
    {
        // Reads CSV as list of rows (each row: header -> value)
        public static List<Dictionary<string, string?>> Read(string csvPath)
        {
            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                IgnoreBlankLines = true,
                TrimOptions = TrimOptions.Trim,
                BadDataFound = null
            });

            csv.Read();
            csv.ReadHeader();

            var headers = csv.HeaderRecord ?? Array.Empty<string>();
            var rows = new List<Dictionary<string, string?>>();

            while (csv.Read())
            {
                var row = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

                foreach (var h in headers)
                {
                    var val = csv.GetField(h);
                    row[h] = string.IsNullOrWhiteSpace(val) ? null : val.Trim();
                }

                // skip fully empty row
                if (row.Values.All(v => v is null))
                    continue;

                rows.Add(row);
            }

            return rows;
        }

        public static string? Get(this Dictionary<string, string?> row, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (row.TryGetValue(key, out var v) && !string.IsNullOrWhiteSpace(v))
                    return v;
            }
            return null;
        }

        public static int GetInt(this Dictionary<string, string?> row, string key)
        {
            var s = row.Get(key) ?? throw new Exception($"{key} is required.");
            if (!int.TryParse(s, out var value))
                throw new Exception($"{key} must be an integer. Value: '{s}'");
            return value;
        }

        public static double GetDouble(this Dictionary<string, string?> row, string key)
        {
            var s = row.Get(key) ?? throw new Exception($"{key} is required.");
            if (!double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
                throw new Exception($"{key} must be a number. Value: '{s}'");
            return value;
        }
    }
}
