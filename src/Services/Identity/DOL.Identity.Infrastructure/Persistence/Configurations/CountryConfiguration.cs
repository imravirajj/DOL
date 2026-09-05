using DOL.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DOL.Identity.Infrastructure.Persistence.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("countries");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CompanyId)
            .IsRequired()
            .HasColumnName("company_id");

        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired()
            .HasColumnName("name");

        builder.Property(c => c.IsoCode)
            .HasMaxLength(10)
            .IsRequired()
            .HasColumnName("iso_code");

        builder.HasIndex(c => new { c.CompanyId, c.IsoCode })
            .IsUnique();

        builder.HasMany(c => c.States)
            .WithOne(s => s.Country)
            .HasForeignKey(s => s.CountryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
