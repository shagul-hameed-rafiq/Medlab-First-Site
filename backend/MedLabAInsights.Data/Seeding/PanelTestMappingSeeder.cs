using MedLabAInsights.Data.Contexts;
using MedLabAInsights.Models;
using Microsoft.EntityFrameworkCore;

namespace MedLabAInsights.Data.Seeding
{
    public static class PanelTestMappingSeeder
    {
        public static async Task SeedAsync(MedlabAinsightDbContext db)
        {
            // Swap Panel 2 (CBC) and Panel 3 (Thyroid) mapping IDs logically
            var seed = new List<PanelTestMapping>
            {
                // PanelId 1 (Diabetic)
                new() { PanelId = 1, TestId = 1, ImportanceLevel = 1 },
                new() { PanelId = 1, TestId = 2, ImportanceLevel = 1 },
                new() { PanelId = 1, TestId = 3, ImportanceLevel = 1 },
                new() { PanelId = 1, TestId = 4, ImportanceLevel = 2 },

                // PanelId 3 (Thyroid)
                // Tests 5, 6, 7 are TSH, Free T3, Free T4
                new() { PanelId = 3, TestId = 5, ImportanceLevel = 1 },
                new() { PanelId = 3, TestId = 6, ImportanceLevel = 1 },
                new() { PanelId = 3, TestId = 7, ImportanceLevel = 1 },

                // PanelId 2 (CBC)
                // Tests 8-17 are CBC tests
                new() { PanelId = 2, TestId = 8,  ImportanceLevel = 1 },
                new() { PanelId = 2, TestId = 9,  ImportanceLevel = 2 },
                new() { PanelId = 2, TestId = 10, ImportanceLevel = 1 },
                new() { PanelId = 2, TestId = 11, ImportanceLevel = 1 },
                new() { PanelId = 2, TestId = 12, ImportanceLevel = 2 },
                new() { PanelId = 2, TestId = 13, ImportanceLevel = 2 },
                new() { PanelId = 2, TestId = 14, ImportanceLevel = 2 },
                new() { PanelId = 2, TestId = 15, ImportanceLevel = 2 },
                new() { PanelId = 2, TestId = 16, ImportanceLevel = 1 },
                new() { PanelId = 2, TestId = 17, ImportanceLevel = 1 },
            };

            // To fix the issue transparently on Render, we clear out any incorrect mappings that were seeded previously
            var existingMappings = await db.PanelTestMappings.ToListAsync();
            db.PanelTestMappings.RemoveRange(existingMappings);
            await db.SaveChangesAsync();

            await db.PanelTestMappings.AddRangeAsync(seed);
            await db.SaveChangesAsync();
        }
    }
}
