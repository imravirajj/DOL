using DOL.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DOL.Identity.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("application_users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .HasMaxLength(100)
            .IsRequired()
            .HasColumnName("first_name");

        builder.Property(u => u.LastName)
            .HasMaxLength(100)
            .IsRequired()
            .HasColumnName("last_name");

        builder.Property(u => u.Email)
            .HasMaxLength(256)
            .IsRequired()
            .HasColumnName("email");

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired()
            .HasColumnName("phone_number");

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasColumnName("password_hash");

        builder.Property(u => u.Status)
            .HasConversion<int>()
            .HasColumnName("status");

        builder.Property(u => u.EmailConfirmed)
            .HasColumnName("email_confirmed");

        builder.Property(u => u.AccessFailedCount)
            .HasColumnName("access_failed_count");

        builder.Property(u => u.LockoutEnd)
            .HasColumnName("lockout_end");

        builder.Property(u => u.PasswordResetToken)
            .HasMaxLength(256)
            .HasColumnName("password_reset_token");

        builder.Property(u => u.PasswordResetTokenExpiresAt)
            .HasColumnName("password_reset_token_expires_at");

        builder.Property(u => u.CompanyId)
            .HasColumnName("company_id");

        builder.Property(u => u.Scope)
            .HasConversion<int>()
            .HasColumnName("access_scope");

        builder.Property(u => u.ScopeEntityId)
            .HasColumnName("scope_entity_id");

        builder.Property(u => u.BranchId)
            .HasColumnName("branch_id");

        builder.HasOne(u => u.Company)
            .WithMany(c => c.Users)
            .HasForeignKey(u => u.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Branch)
            .WithMany(b => b.Users)
            .HasForeignKey(u => u.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(u => u.CreatedBy)
            .HasMaxLength(100)
            .HasColumnName("created_by");

        builder.Property(u => u.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(u => u.UpdatedBy)
            .HasMaxLength(100)
            .HasColumnName("updated_by");

        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.RefreshTokens)
            .WithOne()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
