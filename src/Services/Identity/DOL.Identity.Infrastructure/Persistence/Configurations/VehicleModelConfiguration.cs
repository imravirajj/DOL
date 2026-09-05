using DOL.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DOL.Identity.Infrastructure.Persistence.Configurations;

public class VehicleModelConfiguration : IEntityTypeConfiguration<VehicleModel>
{
    public void Configure(EntityTypeBuilder<VehicleModel> builder)
    {
        builder.ToTable("vehicle_models");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.CompanyId)
            .IsRequired()
            .HasColumnName("company_id");

        builder.Property(m => m.Make)
            .HasMaxLength(100)
            .IsRequired()
            .HasColumnName("make");

        builder.Property(m => m.Model)
            .HasMaxLength(100)
            .IsRequired()
            .HasColumnName("model");

        builder.Property(m => m.Year)
            .HasColumnName("year");

        builder.Property(m => m.Category)
            .HasMaxLength(50)
            .HasColumnName("category");

        builder.Property(m => m.IsActive)
            .HasColumnName("is_active");

        builder.Property(m => m.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(m => m.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasMany(m => m.Variants)
            .WithOne(v => v.VehicleModel)
            .HasForeignKey(v => v.VehicleModelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
