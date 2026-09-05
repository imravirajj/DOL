using DOL.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DOL.Identity.Infrastructure.Persistence.Configurations;

public class VehicleStockConfiguration : IEntityTypeConfiguration<VehicleStock>
{
    public void Configure(EntityTypeBuilder<VehicleStock> builder)
    {
        builder.ToTable("vehicle_stocks");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.CompanyId)
            .IsRequired()
            .HasColumnName("company_id");

        builder.Property(s => s.BranchId)
            .IsRequired()
            .HasColumnName("branch_id");

        builder.Property(s => s.VehicleVariantId)
            .IsRequired()
            .HasColumnName("vehicle_variant_id");

        builder.Property(s => s.VinNumber)
            .HasMaxLength(50)
            .IsRequired()
            .HasColumnName("vin_number");

        builder.HasIndex(s => new { s.CompanyId, s.VinNumber })
            .IsUnique();

        builder.Property(s => s.EngineNumber)
            .HasMaxLength(50)
            .IsRequired()
            .HasColumnName("engine_number");

        builder.Property(s => s.Color)
            .HasMaxLength(50)
            .IsRequired()
            .HasColumnName("color");

        builder.Property(s => s.Status)
            .HasConversion<int>()
            .HasColumnName("status");

        builder.Property(s => s.ReservedByBuyerId)
            .HasColumnName("reserved_by_buyer_id");

        builder.Property(s => s.ReservationExpiresAt)
            .HasColumnName("reservation_expires_at");

        builder.Property(s => s.ConfirmedOrderId)
            .HasColumnName("confirmed_order_id");

        // Optimistic Concurrency Token
        builder.Property(s => s.Version)
            .IsConcurrencyToken()
            .HasColumnName("version");

        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(s => s.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasIndex(s => new { s.BranchId, s.VehicleVariantId, s.Status });
        builder.HasIndex(s => s.ReservationExpiresAt);

        builder.HasOne(s => s.Branch)
            .WithMany()
            .HasForeignKey(s => s.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
