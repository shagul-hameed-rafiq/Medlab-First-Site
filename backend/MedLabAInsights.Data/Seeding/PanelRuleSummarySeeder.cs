using MedLabAInsights.Data.Contexts;
using MedLabAInsights.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MedLabAInsights.Data.Seeding
{
    public static class PanelRuleSummarySeeder
    {
        public static async Task SeedAsync(MedlabAinsightDbContext db, string csvPath)
        {
            var rows = CsvDataLoader.Read(csvPath);

            // Resolve PanelCode -> PanelId (preferred)
            var panelCodeMap = await db.Panels
                .Where(p => p.PanelCode != null)
                .ToDictionaryAsync(p => p.PanelCode!, p => p.PanelId);

            foreach (var r in rows)
            {
                int panelId;
                var panelCode = r.Get("PanelCode");
                if (!string.IsNullOrWhiteSpace(panelCode))
                {
                    if (!panelCodeMap.TryGetValue(panelCode, out panelId))
                        throw new Exception($"Unknown PanelCode '{panelCode}' in CSV: {csvPath}");
                }
                else
                {
                    panelId = r.GetInt("PanelId");
                }

                var ruleCode = r.Get("PanelRuleCode") ?? throw new Exception("PanelRuleCode is required.");
                var ruleName = r.Get("PanelRuleName") ?? throw new Exception("PanelRuleName is required.");

                var minSeverity = r.GetInt("MinSeverity");
                var maxSeverity = r.GetInt("MaxSeverity");

                var summary = r.Get("StandardSummary", "StandartSummary")
                             ?? throw new Exception("StandardSummary is required.");

                // Upsert by (PanelId + PanelRuleCode)
                var existing = await db.PanelRuleSummaries
                    .FirstOrDefaultAsync(x => x.PanelId == panelId && x.PanelRuleCode == ruleCode);

                if (existing == null)
                {
                    db.PanelRuleSummaries.Add(new Models.PanelRuleSummary
                    {
                        PanelId = panelId,
                        PanelRuleCode = ruleCode,
                        PanelRuleName = ruleName,
                        MinSeverity = minSeverity,
                        MaxSeverity = maxSeverity,
                        StandardSummary = summary
                    });
                }
                else
                {
                    existing.PanelRuleName = ruleName;
                    existing.MinSeverity = minSeverity;
                    existing.MaxSeverity = maxSeverity;
                    existing.StandardSummary = summary;
                }
            }

            await db.SaveChangesAsync();
        }
    }
}
