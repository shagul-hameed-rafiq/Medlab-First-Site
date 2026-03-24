using MedLabAInsights.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedLabAInsights.Data.Configurations;

public class VisitTestResultConfiguration : IEntityTypeConfiguration<VisitTestResult>
{
    public void Configure(EntityTypeBuilder<VisitTestResult> builder)
    {
        builder.ToTable("VisitTestResult");

        builder.HasKey(x => x.VisitTestResultId);

        builder.Property(x => x.RawValue)
            .IsRequired()
            .HasMaxLength(200); // adjust if you store long raw strings

        builder.Property(x => x.EnteredAt);

        // Visit (1) -> VisitTestResult (many)
        builder.HasOne(x => x.Visit)
            .WithMany() // change to .WithMany(v => v.TestResults) if you add navigation on Visit
            .HasForeignKey(x => x.VisitId)
            .OnDelete(DeleteBehavior.Cascade);

        // Test (1) -> VisitTestResult (many)
        builder.HasOne(x => x.Test)
            .WithMany() // change to .WithMany(t => t.VisitTestResults) if you add navigation on Test
            .HasForeignKey(x => x.TestId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevent duplicate test rows per visit (recommended)
        builder.HasIndex(x => new { x.VisitId, x.TestId }).IsUnique();

        builder.HasIndex(x => x.TestId);
    }
}
