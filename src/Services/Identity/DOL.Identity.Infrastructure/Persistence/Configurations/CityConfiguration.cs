using DOL.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DOL.Identity.Infrastructure.Persistence.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("cities");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CompanyId)
            .IsRequired()
            .HasColumnName("company_id");

        builder.Property(c => c.StateRegionId)
            .IsRequired()
            .HasColumnName("state_region_id");

        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired()
            .HasColumnName("name");

        builder.HasIndex(c => new { c.StateRegionId, c.Name })
            .IsUnique();

        builder.HasMany(c => c.Branches)
            .WithOne(b => b.City)
            .HasForeignKey(b => b.CityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
