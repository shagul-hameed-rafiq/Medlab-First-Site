using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedLabAInsights.Models;

namespace MedLabAInsights.Data.Configurations
{
    public class PanelConfiguration : IEntityTypeConfiguration<Panel>
    {
        public void Configure(EntityTypeBuilder<Panel> entity)
        {
            entity.ToTable("Panel");

            entity.HasKey(x => x.PanelId);

            entity.Property(x => x.PanelId)
                  .ValueGeneratedOnAdd();

            entity.Property(x => x.PanelName)
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(x => x.PanelCode)
                  .HasMaxLength(50);
        }
    }
}