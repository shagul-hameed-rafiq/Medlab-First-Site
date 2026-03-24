using MedLabAInsights.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedLabAInsights.Data.Configurations
{
    public class PanelRuleSummaryConfiguration : IEntityTypeConfiguration<PanelRuleSummary>
    {
        public void Configure(EntityTypeBuilder<PanelRuleSummary> entity)
        {
            entity.ToTable("PanelRuleSummary");

            entity.HasKey(x => x.PanelRuleId);

            entity.Property(x => x.PanelRuleId)
                  .ValueGeneratedOnAdd();

            entity.Property(x => x.PanelRuleName)
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(x => x.PanelRuleCode)
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(x => x.MinSeverity)
                  .IsRequired();

            entity.Property(x => x.MaxSeverity)
                  .IsRequired();

            entity.Property(x => x.StandardSummary)
                  .HasMaxLength(2000)
                  .IsRequired();

            entity.HasOne(x => x.Panel)
                  .WithMany()
                  .HasForeignKey(x => x.PanelId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Prevent duplicate codes per panel
            entity.HasIndex(x => new { x.PanelId, x.PanelRuleCode })
                  .IsUnique();

            // Helpful for lookup queries
            entity.HasIndex(x => x.PanelId);
        }
    }
}
