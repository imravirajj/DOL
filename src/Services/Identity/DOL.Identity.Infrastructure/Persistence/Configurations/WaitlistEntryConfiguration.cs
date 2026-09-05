using DOL.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DOL.Identity.Infrastructure.Persistence.Configurations;

public class WaitlistEntryConfiguration : IEntityTypeConfiguration<WaitlistEntry>
{
    public void Configure(EntityTypeBuilder<WaitlistEntry> builder)
    {
        builder.ToTable("waitlist_entries");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.CompanyId)
            .IsRequired()
            .HasColumnName("company_id");

        builder.Property(w => w.BranchId)
            .IsRequired()
            .HasColumnName("branch_id");

        builder.Property(w => w.VehicleVariantId)
            .IsRequired()
            .HasColumnName("vehicle_variant_id");

        builder.Property(w => w.BuyerId)
            .IsRequired()
            .HasColumnName("buyer_id");

        builder.Property(w => w.QueuePosition)
            .IsRequired()
            .HasColumnName("queue_position");

        builder.Property(w => w.TokenAmountPaid)
            .HasPrecision(18, 2)
            .HasColumnName("token_amount_paid");

        builder.Property(w => w.IdempotencyKey)
            .HasMaxLength(100)
            .IsRequired()
            .HasColumnName("idempotency_key");

        builder.HasIndex(w => new { w.CompanyId, w.IdempotencyKey })
            .IsUnique();

        builder.Property(w => w.Status)
            .HasConversion<int>()
            .HasColumnName("status");

        builder.Property(w => w.AllocatedStockId)
            .HasColumnName("allocated_stock_id");

        builder.Property(w => w.AllocatedAt)
            .HasColumnName("allocated_at");

        builder.Property(w => w.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(w => w.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasIndex(w => new { w.BranchId, w.VehicleVariantId, w.QueuePosition });

        builder.HasOne(w => w.Branch)
            .WithMany()
            .HasForeignKey(w => w.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.Buyer)
            .WithMany()
            .HasForeignKey(w => w.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
