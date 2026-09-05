using DOL.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DOL.Identity.Infrastructure.Persistence.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("companies");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .HasMaxLength(200)
            .IsRequired()
            .HasColumnName("name");

        builder.Property(c => c.Code)
            .HasMaxLength(50)
            .IsRequired()
            .HasColumnName("code");

        builder.HasIndex(c => c.Code)
            .IsUnique();

        builder.Property(c => c.Email)
            .HasMaxLength(256)
            .IsRequired()
            .HasColumnName("email");

        builder.Property(c => c.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired()
            .HasColumnName("phone_number");

        builder.Property(c => c.Address)
            .HasMaxLength(500)
            .HasColumnName("address");

        builder.Property(c => c.SubscriptionPlan)
            .HasMaxLength(50)
            .IsRequired()
            .HasColumnName("subscription_plan");

        builder.Property(c => c.Status)
            .HasConversion<int>()
            .HasColumnName("status");

        builder.Property(c => c.Currency)
            .HasMaxLength(10)
            .IsRequired()
            .HasColumnName("currency");

        builder.Property(c => c.TimeZone)
            .HasMaxLength(50)
            .IsRequired()
            .HasColumnName("time_zone");

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(c => c.CreatedBy)
            .HasMaxLength(100)
            .HasColumnName("created_by");

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(c => c.UpdatedBy)
            .HasMaxLength(100)
            .HasColumnName("updated_by");
    }
}
