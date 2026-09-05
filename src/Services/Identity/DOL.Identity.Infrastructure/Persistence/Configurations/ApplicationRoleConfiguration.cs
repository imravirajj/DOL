using DOL.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DOL.Identity.Infrastructure.Persistence.Configurations;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable("application_roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .HasMaxLength(50)
            .IsRequired()
            .HasColumnName("name");

        builder.HasIndex(r => r.Name)
            .IsUnique();

        builder.Property(r => r.Description)
            .HasMaxLength(200)
            .HasColumnName("description");

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at");

        // Static seed date (UTC)
        var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var adminRole = new ApplicationRole(ApplicationRole.AdminRoleId, ApplicationRole.Admin, "Platform Administrator", seedDate);
        var buyerRole = new ApplicationRole(ApplicationRole.BuyerRoleId, ApplicationRole.Buyer, "End customer buying vehicles", seedDate);
        var dealerRole = new ApplicationRole(ApplicationRole.DealerRoleId, ApplicationRole.Dealer, "Vehicle seller", seedDate);
        var companyAdminRole = new ApplicationRole(ApplicationRole.CompanyAdminRoleId, ApplicationRole.CompanyAdmin, "Company Super Administrator", seedDate);
        var branchManagerRole = new ApplicationRole(ApplicationRole.BranchManagerRoleId, ApplicationRole.BranchManager, "Branch Manager", seedDate);
        var branchStaffRole = new ApplicationRole(ApplicationRole.BranchStaffRoleId, ApplicationRole.BranchStaff, "Branch Staff Member", seedDate);

        builder.HasData(adminRole, buyerRole, dealerRole, companyAdminRole, branchManagerRole, branchStaffRole);
    }
}
