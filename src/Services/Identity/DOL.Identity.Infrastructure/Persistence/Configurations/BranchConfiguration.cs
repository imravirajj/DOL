using DOL.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DOL.Identity.Infrastructure.Persistence.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("branches");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.CompanyId)
            .IsRequired()
            .HasColumnName("company_id");

        builder.Property(b => b.CityId)
            .IsRequired()
            .HasColumnName("city_id");

        builder.Property(b => b.Name)
            .HasMaxLength(150)
            .IsRequired()
            .HasColumnName("name");

        builder.Property(b => b.BranchCode)
            .HasMaxLength(50)
            .IsRequired()
            .HasColumnName("branch_code");

        builder.HasIndex(b => new { b.CompanyId, b.BranchCode })
            .IsUnique();

        builder.Property(b => b.Address)
            .HasMaxLength(500)
            .IsRequired()
            .HasColumnName("address");

        builder.Property(b => b.ContactPhone)
            .HasMaxLength(20)
            .HasColumnName("contact_phone");

        builder.Property(b => b.ContactEmail)
            .HasMaxLength(256)
            .HasColumnName("contact_email");

        builder.Property(b => b.IsActive)
            .HasColumnName("is_active");

        builder.Property(b => b.IsMainBranch)
            .HasColumnName("is_main_branch");

        builder.Property(b => b.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(b => b.CreatedBy)
            .HasMaxLength(100)
            .HasColumnName("created_by");

        builder.Property(b => b.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(b => b.UpdatedBy)
            .HasMaxLength(100)
            .HasColumnName("updated_by");

        builder.HasOne(b => b.Company)
            .WithMany(c => c.Branches)
            .HasForeignKey(b => b.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.City)
            .WithMany(c => c.Branches)
            .HasForeignKey(b => b.CityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
