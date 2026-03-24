using MedLabAInsights.Data.Contexts;
using MedLabAInsights.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MedLabAInsights.Data.Seeding
{
    public static class BandRuleReportSeeder
    {
        public static async Task SeedAsync(MedlabAinsightDbContext db, string csvPath)
        {
            var rows = CsvDataLoader.Read(csvPath);

            // Resolve TestCode -> TestId (preferred)
            var testCodeMap = await db.Tests
                .Where(t => t.TestCode != null)
                .ToDictionaryAsync(t => t.TestCode!, t => t.TestId);

            foreach (var r in rows)
            {
                // Resolve TestId (prefer TestCode; fallback TestId)
                int testId;
                var testCode = r.Get("TestCode");
                if (!string.IsNullOrWhiteSpace(testCode))
                {
                    if (!testCodeMap.TryGetValue(testCode, out testId))
                        throw new Exception($"Unknown TestCode '{testCode}' in CSV: {csvPath}");
                }
                else
                {
                    testId = r.GetInt("TestId");
                }

                var bandCode = r.Get("BandCode") ?? throw new Exception("BandCode is required.");
                var bandName = r.Get("BandName") ?? throw new Exception("BandName is required.");

                var rangeMin = r.GetDouble("RangeMin");
                var rangeMax = r.GetDouble("RangeMax");
                var severity = r.GetInt("Severity");

                var customValue = r.Get("CustomValue", "CustomValues");
                var standardReport = r.Get("StandardReport") ?? throw new Exception("StandardReport is required.");

                // Upsert by (TestId + BandCode)
                var existing = await db.BandRuleReports
                    .FirstOrDefaultAsync(x => x.TestId == testId && x.BandCode == bandCode);

                if (existing == null)
                {
                    db.BandRuleReports.Add(new Models.BandRuleReport
                    {
                        TestId = testId,
                        BandCode = bandCode,
                        BandName = bandName,
                        RangeMin = rangeMin,
                        RangeMax = rangeMax,
                        Severity = severity,
                        CustomValue = customValue,
                        StandardReport = standardReport
                    });
                }
                else
                {
                    existing.BandName = bandName;
                    existing.RangeMin = rangeMin;
                    existing.RangeMax = rangeMax;
                    existing.Severity = severity;
                    existing.CustomValue = customValue;
                    existing.StandardReport = standardReport;
                }
            }

            await db.SaveChangesAsync();
        }
    }
}
