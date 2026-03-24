using MedLabAInsights.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedLabAInsights.Data.Configurations
{
    public class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> entity)
        {
            entity.ToTable("Member");

            entity.HasKey(x => x.MemberId);

            entity.Property(x => x.MemberId)
                  .ValueGeneratedOnAdd();

            entity.Property(x => x.Name)
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(x => x.Gender)
                  .IsRequired();

            entity.Property(x => x.DateOfBirth)
                  .IsRequired();

            entity.Property(x => x.BloodGroup)
                  .IsRequired();

            entity.Property(x => x.Contact)
                  .IsRequired();

            entity.Property(x => x.Address)
                  .HasMaxLength(500);

            // Optional but useful: prevent duplicate phone numbers
            entity.HasIndex(x => x.Contact)
                  .IsUnique();
        }
    }
}
