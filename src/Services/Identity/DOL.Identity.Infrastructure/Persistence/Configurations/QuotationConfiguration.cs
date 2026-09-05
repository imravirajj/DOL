using DOL.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DOL.Identity.Infrastructure.Persistence.Configurations;

public class QuotationConfiguration : IEntityTypeConfiguration<Quotation>
{
    public void Configure(EntityTypeBuilder<Quotation> builder)
    {
        builder.ToTable("quotations");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.CompanyId)
            .IsRequired()
            .HasColumnName("company_id");

        builder.Property(q => q.BranchId)
            .IsRequired()
            .HasColumnName("branch_id");

        builder.Property(q => q.VehicleVariantId)
            .IsRequired()
            .HasColumnName("vehicle_variant_id");

        builder.Property(q => q.BuyerId)
            .HasColumnName("buyer_id");

        builder.Property(q => q.QuotationNumber)
            .HasMaxLength(50)
            .IsRequired()
            .HasColumnName("quotation_number");

        builder.HasIndex(q => new { q.CompanyId, q.QuotationNumber })
            .IsUnique();

        builder.Property(q => q.CustomerName)
            .HasMaxLength(150)
            .IsRequired()
            .HasColumnName("customer_name");

        builder.Property(q => q.CustomerEmail)
            .HasMaxLength(256)
            .IsRequired()
            .HasColumnName("customer_email");

        builder.Property(q => q.CustomerPhone)
            .HasMaxLength(20)
            .IsRequired()
            .HasColumnName("customer_phone");

        builder.Property(q => q.SelectedColor)
            .HasMaxLength(50)
            .IsRequired()
            .HasColumnName("selected_color");

        // Monetary fields
        builder.Property(q => q.ExShowroomPrice).HasPrecision(18, 2).HasColumnName("ex_showroom_price");
        builder.Property(q => q.RtoTaxAmount).HasPrecision(18, 2).HasColumnName("rto_tax_amount");
        builder.Property(q => q.InsuranceBaseAmount).HasPrecision(18, 2).HasColumnName("insurance_base_amount");
        builder.Property(q => q.InsuranceAddonsAmount).HasPrecision(18, 2).HasColumnName("insurance_addons_amount");
        builder.Property(q => q.FastagCharges).HasPrecision(18, 2).HasColumnName("fastag_charges");
        builder.Property(q => q.TcsAmount).HasPrecision(18, 2).HasColumnName("tcs_amount");
        builder.Property(q => q.AccessoriesTotal).HasPrecision(18, 2).HasColumnName("accessories_total");
        builder.Property(q => q.ExtendedWarrantyAmount).HasPrecision(18, 2).HasColumnName("extended_warranty_amount");
        builder.Property(q => q.DiscountAmount).HasPrecision(18, 2).HasColumnName("discount_amount");
        builder.Property(q => q.TotalOnRoadPrice).HasPrecision(18, 2).HasColumnName("total_on_road_price");

        builder.Property(q => q.IncludeZeroDep).HasColumnName("include_zero_dep");
        builder.Property(q => q.IncludeEngineProtect).HasColumnName("include_engine_protect");
        builder.Property(q => q.IncludeReturnToInvoice).HasColumnName("include_return_to_invoice");
        builder.Property(q => q.IncludeExtendedWarranty).HasColumnName("include_extended_warranty");
        builder.Property(q => q.SelectedAccessoriesJson).HasColumnName("selected_accessories_json");

        builder.Property(q => q.Status).HasConversion<int>().HasColumnName("status");
        builder.Property(q => q.ValidUntil).HasColumnName("valid_until");

        builder.Property(q => q.CreatedAt).HasColumnName("created_at");
        builder.Property(q => q.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(q => new { q.BranchId, q.Status });
        builder.HasIndex(q => q.CustomerEmail);

        builder.HasOne(q => q.Branch)
            .WithMany()
            .HasForeignKey(q => q.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(q => q.VehicleVariant)
            .WithMany()
            .HasForeignKey(q => q.VehicleVariantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(q => q.Buyer)
            .WithMany()
            .HasForeignKey(q => q.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
