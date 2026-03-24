using MedLabAInsights.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedLabAInsights.Data.Configurations;

public class VisitConfiguration : IEntityTypeConfiguration<Visit>
{
    public void Configure(EntityTypeBuilder<Visit> builder)
    {
        builder.ToTable("Visit");

        builder.HasKey(x => x.VisitId);

        builder.Property(x => x.VisitDateTime)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        // Member (1) -> Visit (many)
        builder.HasOne(x => x.Member)
            .WithMany() // change to .WithMany(m => m.Visits) if you add navigation on Member
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        // Panel (1) -> Visit (many)
        builder.HasOne(x => x.Panel)
            .WithMany() // change to .WithMany(p => p.Visits) if you add navigation on Panel
            .HasForeignKey(x => x.PanelId)
            .OnDelete(DeleteBehavior.Restrict);

        // Common query patterns
        builder.HasIndex(x => new { x.MemberId, x.VisitDateTime });
        builder.HasIndex(x => x.PanelId);
    }
}
