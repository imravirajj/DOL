using DOL.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DOL.Identity.Infrastructure.Persistence.Configurations;

public class VehicleVariantConfiguration : IEntityTypeConfiguration<VehicleVariant>
{
    public void Configure(EntityTypeBuilder<VehicleVariant> builder)
    {
        builder.ToTable("vehicle_variants");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.CompanyId)
            .IsRequired()
            .HasColumnName("company_id");

        builder.Property(v => v.VehicleModelId)
            .IsRequired()
            .HasColumnName("vehicle_model_id");

        builder.Property(v => v.VariantName)
            .HasMaxLength(150)
            .IsRequired()
            .HasColumnName("variant_name");

        builder.Property(v => v.FuelType)
            .HasMaxLength(50)
            .IsRequired()
            .HasColumnName("fuel_type");

        builder.Property(v => v.Transmission)
            .HasMaxLength(50)
            .IsRequired()
            .HasColumnName("transmission");

        builder.Property(v => v.ExShowroomPrice)
            .HasPrecision(18, 2)
            .IsRequired()
            .HasColumnName("ex_showroom_price");

        builder.Property(v => v.ColorsAvailable)
            .HasMaxLength(300)
            .HasColumnName("colors_available");

        builder.Property(v => v.IsActive)
            .HasColumnName("is_active");

        builder.Property(v => v.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(v => v.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasMany(v => v.StockUnits)
            .WithOne(s => s.VehicleVariant)
            .HasForeignKey(s => s.VehicleVariantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
