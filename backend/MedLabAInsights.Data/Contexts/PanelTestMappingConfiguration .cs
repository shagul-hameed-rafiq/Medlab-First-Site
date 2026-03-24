using MedLabAInsights.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedLabAInsights.Data.Configurations
{
    public class PanelTestMappingConfiguration : IEntityTypeConfiguration<PanelTestMapping>
    {
        public void Configure(EntityTypeBuilder<PanelTestMapping> entity)
        {
            entity.ToTable("PanelTestMapping");

            entity.HasKey(x => x.PanelTestId);

            entity.Property(x => x.PanelTestId)
                  .ValueGeneratedOnAdd();

            entity.Property(x => x.ImportanceLevel)
                  .IsRequired();

            entity.HasOne(x => x.Panel)
                  .WithMany()                 // we can replace with .WithMany(p => p.PanelTestMappings) if you add navigation
                  .HasForeignKey(x => x.PanelId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Test)
                  .WithMany()                 // we can replace with .WithMany(t => t.PanelTestMappings)
                  .HasForeignKey(x => x.TestId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Prevent duplicates: same PanelId + TestId should not repeat
            entity.HasIndex(x => new { x.PanelId, x.TestId })
                  .IsUnique();
        }
    }
}
