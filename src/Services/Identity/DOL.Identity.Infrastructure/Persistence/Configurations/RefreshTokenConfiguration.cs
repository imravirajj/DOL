using DOL.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DOL.Identity.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.UserId).HasColumnName("user_id");

        builder.Property(rt => rt.Token)
            .HasMaxLength(500)
            .IsRequired()
            .HasColumnName("token");

        builder.HasIndex(rt => rt.Token)
            .IsUnique();

        builder.Property(rt => rt.ExpiresAt).HasColumnName("expires_at");
        builder.Property(rt => rt.CreatedAt).HasColumnName("created_at");
        builder.Property(rt => rt.CreatedByIp).HasMaxLength(50).HasColumnName("created_by_ip");
        builder.Property(rt => rt.RevokedAt).HasColumnName("revoked_at");
        builder.Property(rt => rt.ReplacedByToken).HasMaxLength(500).HasColumnName("replaced_by_token");
    }
}
