using MedLabAInsights.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedLabAInsights.Data.Configurations;

public class VisitPanelSummaryConfiguration : IEntityTypeConfiguration<VisitPanelSummary>
{
    public void Configure(EntityTypeBuilder<VisitPanelSummary> builder)
    {
        builder.ToTable("VisitPanelSummary");

        builder.HasKey(x => x.VisitPanelSummaryId);

        builder.Property(x => x.StandardSummarySnapshot)
            .IsRequired()
            .HasColumnType("TEXT");

        builder.Property(x => x.RevisedSummary)
            .HasColumnType("TEXT");

        builder.Property(x => x.IsRevised)
            .IsRequired();

        builder.Property(x => x.EvaluatedAt)
            .IsRequired();

        // Visit (1) -> PanelSummary (0..1 or many?)
        builder.HasOne(x => x.Visit)
            .WithMany() // change to .WithMany(v => v.PanelSummaries) if you add nav
            .HasForeignKey(x => x.VisitId)
            .OnDelete(DeleteBehavior.Cascade);

        // Recommended: only one summary per visit (for the chosen panel)
        builder.HasIndex(x => x.VisitId).IsUnique();

        // PanelRuleSummary
        builder.HasOne(x => x.PanelRuleSummary)
            .WithMany() // change to .WithMany(r => r.VisitPanelSummaries) if nav exists
            .HasForeignKey(x => x.PanelRuleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.PanelRuleId);
        builder.HasIndex(x => x.EvaluatedAt);
    }
}
