using MedLabAInsights.Data.Contexts;
using MedLabAInsights.Models;
using Microsoft.EntityFrameworkCore;

namespace MedLabAInsights.Data.Seeding
{
    public static class TestSeeder
    {
        public static async Task SeedAsync(MedlabAinsightDbContext db)
        {
            var seed = new List<Test>
            {
                new() { TestName = "HbA1c", TestCode = "DIA_HBA1C", MinValue = 4, MaxValue = 5.6, Unit = "%" },
                new() { TestName = "FBS", TestCode = "DIA_FBS", MinValue = 70, MaxValue = 100, Unit = "mg/dl" },
                new() { TestName = "PPBS", TestCode = "DIA_PPBS", MinValue = 80, MaxValue = 140, Unit = "mg/dl" },
                new() { TestName = "Mean Blood Glucose Level", TestCode = "DIA_MBGL", MinValue = 70, MaxValue = 150, Unit = "mg/dl" },

                new() { TestName = "TSH", TestCode = "THY_TSH", MinValue = 0.4, MaxValue = 4.5, Unit = "uIU/mL" },
                new() { TestName = "Free T3", TestCode = "THY_FT3", MinValue = 2, MaxValue = 4.4, Unit = "pg/ml" },
                new() { TestName = "Free T4", TestCode = "THY_FT4", MinValue = 0.8, MaxValue = 1.8, Unit = "ng/dl" },

                new() { TestName = "Hemoglobin", TestCode = "CBC_HB", MinValue = 12, MaxValue = 16, Unit = "g/dl" },
                new() { TestName = "RBC count", TestCode = "CBC_RBC", MinValue = 4, MaxValue = 6, Unit = "million/µL" },
                new() { TestName = "WBC Count", TestCode = "CBC_WBC", MinValue = 4000, MaxValue = 11000, Unit = "cells/µL" },
                new() { TestName = "Platelet Count", TestCode = "CBC_PLT", MinValue = 150000, MaxValue = 450000, Unit = "cells/µL" },

                new() { TestName = "MVC", TestCode = "CBC_MCV", MinValue = 80, MaxValue = 100, Unit = "fL" },
                new() { TestName = "MCH", TestCode = "CBC_MCH", MinValue = 27, MaxValue = 33, Unit = "pg/ml" },
                new() { TestName = "MCHC", TestCode = "CBC_MCHC", MinValue = 32, MaxValue = 36, Unit = "g/dl" },
                new() { TestName = "RWD-CV", TestCode = "CBC_RDW", MinValue = 11.5, MaxValue = 15.0, Unit = "%" },

                new() { TestName = "Neutrophils", TestCode = "CBC_NEU", MinValue = 40, MaxValue = 70, Unit = "%" },
                new() { TestName = "Lymphocytes", TestCode = "CBC_LYM", MinValue = 20, MaxValue = 40, Unit = "%" }
            };

            // Insert only missing TestCode values (best practice)
            var existingCodes = await db.Tests
                .Select(x => x.TestCode!)
                .ToListAsync();

            var toInsert = seed
                .Where(x => !existingCodes.Contains(x.TestCode))
                .ToList();

            if (!toInsert.Any())
                return;

            await db.Tests.AddRangeAsync(toInsert);
            await db.SaveChangesAsync();
        }
    }
}
