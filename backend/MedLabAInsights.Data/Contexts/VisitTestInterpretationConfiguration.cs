using MedLabAInsights.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedLabAInsights.Data.Configurations;

public class VisitTestInterpretationConfiguration : IEntityTypeConfiguration<VisitTestInterpretation>
{
    public void Configure(EntityTypeBuilder<VisitTestInterpretation> builder)
    {
        builder.ToTable("VisitTestInterpretation");

        builder.HasKey(x => x.VisitTestInterpretationId);

        builder.Property(x => x.StandardReportSnapshot)
            .IsRequired()
            .HasColumnType("TEXT"); // SQLite: TEXT for large content

        builder.Property(x => x.RevisedReport)
            .HasColumnType("TEXT");

        builder.Property(x => x.IsRevised)
            .IsRequired();

        builder.Property(x => x.EvaluatedAt)
            .IsRequired();

        // VisitTestResult (1) -> Interpretation (0..1 or many?)
        // If you want exactly ONE interpretation per test-result, keep it Unique.
        builder.HasOne(x => x.VisitTestResult)
            .WithMany() // change to .WithMany(r => r.Interpretations) if you support history
            .HasForeignKey(x => x.VisitTestResultId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.VisitTestResultId).IsUnique(); // enforces one interpretation per result

        // BandRuleReport
        builder.HasOne(x => x.BandRuleReport)
            .WithMany() // change to .WithMany(b => b.VisitTestInterpretations) if nav exists
            .HasForeignKey(x => x.BandId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.BandId);
        builder.HasIndex(x => x.EvaluatedAt);
    }
}
