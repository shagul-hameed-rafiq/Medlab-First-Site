using MedLabAInsights.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedLabAInsights.Data.Configurations
{
    public class BandRuleReportConfiguration : IEntityTypeConfiguration<BandRuleReport>
    {
        public void Configure(EntityTypeBuilder<BandRuleReport> entity)
        {
            entity.ToTable("BandRuleReport");

            entity.HasKey(x => x.BandId);

            entity.Property(x => x.BandId)
                  .ValueGeneratedOnAdd();

            entity.Property(x => x.TestId)
                  .IsRequired();

            entity.Property(x => x.BandName)
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(x => x.BandCode)
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(x => x.RangeMin)
                  .IsRequired();

            entity.Property(x => x.RangeMax)
                  .IsRequired();

            entity.Property(x => x.Severity)
                  .IsRequired();

            entity.Property(x => x.CustomValue)
                  .HasMaxLength(200);

            entity.Property(x => x.StandardReport)
                  .HasMaxLength(2000)
                  .IsRequired();

            entity.HasOne(x => x.Test)
                  .WithMany()
                  .HasForeignKey(x => x.TestId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Prevent duplicate band definitions for same test
            entity.HasIndex(x => new { x.TestId, x.BandCode })
                  .IsUnique();

            // Helpful for query performance
            entity.HasIndex(x => x.TestId);
        }
    }
}
