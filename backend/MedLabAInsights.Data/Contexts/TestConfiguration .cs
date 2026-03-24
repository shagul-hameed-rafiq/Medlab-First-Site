using MedLabAInsights.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedLabAInsights.Data.Configurations
{
    public class TestConfiguration : IEntityTypeConfiguration<Test>
    {
        public void Configure(EntityTypeBuilder<Test> entity)
        {
            entity.ToTable("Test");

            entity.HasKey(x => x.TestId);

            entity.Property(x => x.TestId)
                  .ValueGeneratedOnAdd();

            entity.Property(x => x.TestName)
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(x => x.TestCode)
                  .HasMaxLength(50);

            entity.Property(x => x.CustomValues)
                  .HasMaxLength(500);

            entity.Property(x => x.Unit)
                  .HasMaxLength(50);

            // Optional but recommended (helps avoid duplicates in admin/data imports)
            entity.HasIndex(x => x.TestName).IsUnique();
        }
    }
}
