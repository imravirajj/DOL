using DOL.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DOL.Identity.Infrastructure.Persistence.Configurations;

public class RtoTaxSlabConfiguration : IEntityTypeConfiguration<RtoTaxSlab>
{
    public void Configure(EntityTypeBuilder<RtoTaxSlab> builder)
    {
        builder.ToTable("rto_tax_slabs");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.CompanyId)
            .IsRequired()
            .HasColumnName("company_id");

        builder.Property(r => r.StateName)
            .HasMaxLength(100)
            .IsRequired()
            .HasColumnName("state_name");

        builder.Property(r => r.FuelType)
            .HasMaxLength(50)
            .IsRequired()
            .HasColumnName("fuel_type");

        builder.Property(r => r.TaxPercentage)
            .HasPrecision(5, 2)
            .IsRequired()
            .HasColumnName("tax_percentage");

        builder.Property(r => r.CessPercentage)
            .HasPrecision(5, 2)
            .HasColumnName("cess_percentage");

        builder.HasIndex(r => new { r.CompanyId, r.StateName, r.FuelType })
            .IsUnique();
    }
}
