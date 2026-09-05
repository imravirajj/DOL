using DOL.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DOL.Identity.Infrastructure.Persistence.Configurations;

public class StateRegionConfiguration : IEntityTypeConfiguration<StateRegion>
{
    public void Configure(EntityTypeBuilder<StateRegion> builder)
    {
        builder.ToTable("state_regions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.CompanyId)
            .IsRequired()
            .HasColumnName("company_id");

        builder.Property(s => s.CountryId)
            .IsRequired()
            .HasColumnName("country_id");

        builder.Property(s => s.Name)
            .HasMaxLength(100)
            .IsRequired()
            .HasColumnName("name");

        builder.Property(s => s.StateCode)
            .HasMaxLength(20)
            .HasColumnName("state_code");

        builder.HasIndex(s => new { s.CountryId, s.Name })
            .IsUnique();

        builder.HasMany(s => s.Cities)
            .WithOne(c => c.StateRegion)
            .HasForeignKey(c => c.StateRegionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
