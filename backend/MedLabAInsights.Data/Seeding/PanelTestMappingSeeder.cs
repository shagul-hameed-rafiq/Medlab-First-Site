using MedLabAInsights.Data.Contexts;
using MedLabAInsights.Models;
using Microsoft.EntityFrameworkCore;

namespace MedLabAInsights.Data.Seeding
{
    public static class PanelTestMappingSeeder
    {
        public static async Task SeedAsync(MedlabAinsightDbContext db)
        {
            var seed = new List<PanelTestMapping>
            {
                // PanelId 1
                new() { PanelId = 1, TestId = 1, ImportanceLevel = 1 },
                new() { PanelId = 1, TestId = 2, ImportanceLevel = 1 },
                new() { PanelId = 1, TestId = 3, ImportanceLevel = 1 },
                new() { PanelId = 1, TestId = 4, ImportanceLevel = 2 },

                // PanelId 2
                new() { PanelId = 2, TestId = 5, ImportanceLevel = 1 },
                new() { PanelId = 2, TestId = 6, ImportanceLevel = 1 },
                new() { PanelId = 2, TestId = 7, ImportanceLevel = 1 },

                // PanelId 3
                new() { PanelId = 3, TestId = 8,  ImportanceLevel = 1 },
                new() { PanelId = 3, TestId = 9,  ImportanceLevel = 2 },
                new() { PanelId = 3, TestId = 10, ImportanceLevel = 1 },
                new() { PanelId = 3, TestId = 11, ImportanceLevel = 1 },
                new() { PanelId = 3, TestId = 12, ImportanceLevel = 2 },
                new() { PanelId = 3, TestId = 13, ImportanceLevel = 2 },
                new() { PanelId = 3, TestId = 14, ImportanceLevel = 2 },
                new() { PanelId = 3, TestId = 15, ImportanceLevel = 2 },
                new() { PanelId = 3, TestId = 16, ImportanceLevel = 1 },
                new() { PanelId = 3, TestId = 17, ImportanceLevel = 1 },
            };

            // Insert only missing mappings (PanelId+TestId is unique)
            var existing = await db.PanelTestMappings
                .Select(x => new { x.PanelId, x.TestId })
                .ToListAsync();

            var toInsert = seed
                .Where(x => !existing.Any(e => e.PanelId == x.PanelId && e.TestId == x.TestId))
                .ToList();

            if (!toInsert.Any())
                return;

            await db.PanelTestMappings.AddRangeAsync(toInsert);
            await db.SaveChangesAsync();
        }
    }
}
