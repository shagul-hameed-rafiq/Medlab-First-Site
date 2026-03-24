using MedLabAInsights.Data.Contexts;
using MedLabAInsights.Data.Infrastructure;

namespace MedLabAInsights.Data.Seeding
{
    public static class Seeder
    {
        public static async Task SeedAsync(MedlabAinsightDbContext db)
        {
            await PanelSeeder.SeedAsync(db);
            await TestSeeder.SeedAsync(db);
            await PanelTestMappingSeeder.SeedAsync(db);

            var bandCsv = SourcePath.GetFilePath(@"CSVs\BandRuleReport.csv");
            var panelRuleCsv = SourcePath.GetFilePath(@"CSVs\PanelRuleSummary.csv");

            await BandRuleReportSeeder.SeedAsync(db, bandCsv);
            await PanelRuleSummarySeeder.SeedAsync(db, panelRuleCsv);
        }
    }
}
