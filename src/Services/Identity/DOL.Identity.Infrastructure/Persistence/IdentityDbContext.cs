using System.Reflection;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Infrastructure.Persistence;

public class IdentityDbContext : DbContext, IIdentityDbContext
{
    private readonly ICurrentUserContext? _currentUserContext;

    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<ApplicationRole> Roles => Set<ApplicationRole>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<StateRegion> StateRegions => Set<StateRegion>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<Branch> Branches => Set<Branch>();

    public DbSet<VehicleModel> VehicleModels => Set<VehicleModel>();
    public DbSet<VehicleVariant> VehicleVariants => Set<VehicleVariant>();
    public DbSet<VehicleStock> VehicleStocks => Set<VehicleStock>();
    public DbSet<WaitlistEntry> WaitlistEntries => Set<WaitlistEntry>();
    public DbSet<Quotation> Quotations => Set<Quotation>();
    public DbSet<RtoTaxSlab> RtoTaxSlabs => Set<RtoTaxSlab>();
    public DbSet<LoanApplication> LoanApplications => Set<LoanApplication>();
    public DbSet<VehicleOrder> VehicleOrders => Set<VehicleOrder>();
    public DbSet<TestDriveBooking> TestDriveBookings => Set<TestDriveBooking>();
    public DbSet<DeliveryInspection> DeliveryInspections => Set<DeliveryInspection>();

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options, ICurrentUserContext? currentUserContext = null)
        : base(options)
    {
        _currentUserContext = currentUserContext;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Global Query Filters for Data Isolation:
        // 1. Branch Data Isolation:
        // Non-HQ users can only see their own branch's data.
        modelBuilder.Entity<Branch>().HasQueryFilter(b =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            (
                b.CompanyId == _currentUserContext.CompanyId &&
                (
                    _currentUserContext.IsCompanyAdmin ||
                    _currentUserContext.AccessScope == "CompanyLevel" ||
                    b.Id == _currentUserContext.BranchId
                )
            )
        );

        // 2. Geographic Hierarchy & Tenant Data Isolation:
        modelBuilder.Entity<Country>().HasQueryFilter(c =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            c.CompanyId == _currentUserContext.CompanyId
        );

        modelBuilder.Entity<StateRegion>().HasQueryFilter(s =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            s.CompanyId == _currentUserContext.CompanyId
        );

        modelBuilder.Entity<City>().HasQueryFilter(c =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            c.CompanyId == _currentUserContext.CompanyId
        );

        // 3. ApplicationUser Isolation:
        // Users can only view users belonging to their own company/branch unless platform admin.
        modelBuilder.Entity<ApplicationUser>().HasQueryFilter(u =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            (
                u.CompanyId == _currentUserContext.CompanyId &&
                (
                    _currentUserContext.IsCompanyAdmin ||
                    _currentUserContext.AccessScope == "CompanyLevel" ||
                    u.BranchId == _currentUserContext.BranchId
                )
            )
        );

        modelBuilder.Entity<UserRole>().HasQueryFilter(ur =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            (
                ur.User!.CompanyId == _currentUserContext.CompanyId &&
                (
                    _currentUserContext.IsCompanyAdmin ||
                    _currentUserContext.AccessScope == "CompanyLevel" ||
                    ur.User.BranchId == _currentUserContext.BranchId
                )
            )
        );

        // 4. Vehicle Stock Isolation:
        // Branch staff can only manage their branch's stock; Buyers and HQ can browse across branches in tenant.
        modelBuilder.Entity<VehicleStock>().HasQueryFilter(s =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            (
                s.CompanyId == _currentUserContext.CompanyId &&
                (
                    _currentUserContext.IsCompanyAdmin ||
                    _currentUserContext.AccessScope == "CompanyLevel" ||
                    _currentUserContext.Roles.Contains("Buyer") ||
                    s.BranchId == _currentUserContext.BranchId
                )
            )
        );

        // 5. Waitlist Queue Isolation:
        // Branch staff can only see their branch's waitlist; Buyers can see their own waitlist entries; HQ sees all.
        modelBuilder.Entity<WaitlistEntry>().HasQueryFilter(w =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            (
                w.CompanyId == _currentUserContext.CompanyId &&
                (
                    _currentUserContext.IsCompanyAdmin ||
                    _currentUserContext.AccessScope == "CompanyLevel" ||
                    w.BuyerId == _currentUserContext.UserId ||
                    w.BranchId == _currentUserContext.BranchId
                )
            )
        );

        // 6. Quotation Isolation:
        // Branch staff can only see their branch's quotations; Buyers see their own quotations; HQ sees all.
        modelBuilder.Entity<Quotation>().HasQueryFilter(q =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            (
                q.CompanyId == _currentUserContext.CompanyId &&
                (
                    _currentUserContext.IsCompanyAdmin ||
                    _currentUserContext.AccessScope == "CompanyLevel" ||
                    q.BuyerId == _currentUserContext.UserId ||
                    q.BranchId == _currentUserContext.BranchId
                )
            )
        );

        // 7. RTO Tax Slab Isolation:
        modelBuilder.Entity<RtoTaxSlab>().HasQueryFilter(r =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            r.CompanyId == _currentUserContext.CompanyId
        );

        // 8. Loan Application Isolation:
        modelBuilder.Entity<LoanApplication>().HasQueryFilter(l =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            (
                l.CompanyId == _currentUserContext.CompanyId &&
                (
                    _currentUserContext.IsCompanyAdmin ||
                    _currentUserContext.AccessScope == "CompanyLevel" ||
                    l.BuyerId == _currentUserContext.UserId ||
                    l.BranchId == _currentUserContext.BranchId
                )
            )
        );

        // 9. Vehicle Order Isolation:
        modelBuilder.Entity<VehicleOrder>().HasQueryFilter(o =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            (
                o.CompanyId == _currentUserContext.CompanyId &&
                (
                    _currentUserContext.IsCompanyAdmin ||
                    _currentUserContext.AccessScope == "CompanyLevel" ||
                    o.BuyerId == _currentUserContext.UserId ||
                    o.BranchId == _currentUserContext.BranchId
                )
            )
        );

        // 10. Test Drive Booking Isolation:
        modelBuilder.Entity<TestDriveBooking>().HasQueryFilter(t =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            (
                t.CompanyId == _currentUserContext.CompanyId &&
                (
                    _currentUserContext.IsCompanyAdmin ||
                    _currentUserContext.AccessScope == "CompanyLevel" ||
                    t.BuyerId == _currentUserContext.UserId ||
                    t.BranchId == _currentUserContext.BranchId
                )
            )
        );

        // 11. Delivery Inspection Isolation:
        modelBuilder.Entity<DeliveryInspection>().HasQueryFilter(d =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            (
                d.CompanyId == _currentUserContext.CompanyId &&
                (
                    _currentUserContext.IsCompanyAdmin ||
                    _currentUserContext.AccessScope == "CompanyLevel" ||
                    d.BranchId == _currentUserContext.BranchId
                )
            )
        );
    }
}
