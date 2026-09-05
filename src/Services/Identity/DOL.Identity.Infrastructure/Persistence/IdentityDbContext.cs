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
    public DbSet<VehicleTradeIn> VehicleTradeIns => Set<VehicleTradeIn>();
    public DbSet<VehicleAccessory> VehicleAccessories => Set<VehicleAccessory>();
    public DbSet<InsurancePolicy> InsurancePolicies => Set<InsurancePolicy>();
    public DbSet<ServiceAppointment> ServiceAppointments => Set<ServiceAppointment>();
    public DbSet<CustomerNotification> CustomerNotifications => Set<CustomerNotification>();
    public DbSet<DealershipReview> DealershipReviews => Set<DealershipReview>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<CustomerDocument> CustomerDocuments => Set<CustomerDocument>();
    public DbSet<WarrantyPackage> WarrantyPackages => Set<WarrantyPackage>();
    public DbSet<VehicleWarrantySubscription> VehicleWarrantySubscriptions => Set<VehicleWarrantySubscription>();
    public DbSet<SalesLead> SalesLeads => Set<SalesLead>();
    public DbSet<EvChargingStation> EvChargingStations => Set<EvChargingStation>();
    public DbSet<HomeChargerInstallation> HomeChargerInstallations => Set<HomeChargerInstallation>();

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

        // 12. Vehicle Trade-In Isolation:
        modelBuilder.Entity<VehicleTradeIn>().HasQueryFilter(ti =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            (
                ti.CompanyId == _currentUserContext.CompanyId &&
                (
                    _currentUserContext.IsCompanyAdmin ||
                    _currentUserContext.AccessScope == "CompanyLevel" ||
                    ti.BuyerId == _currentUserContext.UserId ||
                    ti.BranchId == _currentUserContext.BranchId
                )
            )
        );

        // 13. Vehicle Accessory Isolation:
        modelBuilder.Entity<VehicleAccessory>().HasQueryFilter(va =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            va.CompanyId == _currentUserContext.CompanyId
        );

        // 14. Insurance Policy Isolation:
        modelBuilder.Entity<InsurancePolicy>().HasQueryFilter(ip =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            (
                ip.CompanyId == _currentUserContext.CompanyId &&
                (
                    _currentUserContext.IsCompanyAdmin ||
                    _currentUserContext.AccessScope == "CompanyLevel" ||
                    ip.BuyerId == _currentUserContext.UserId ||
                    ip.BranchId == _currentUserContext.BranchId
                )
            )
        );

        // 15. Service Appointment Isolation:
        modelBuilder.Entity<ServiceAppointment>().HasQueryFilter(sa =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            (
                sa.CompanyId == _currentUserContext.CompanyId &&
                (
                    _currentUserContext.IsCompanyAdmin ||
                    _currentUserContext.AccessScope == "CompanyLevel" ||
                    sa.BuyerId == _currentUserContext.UserId ||
                    sa.BranchId == _currentUserContext.BranchId
                )
            )
        );

        // 16. Customer Notification Isolation:
        modelBuilder.Entity<CustomerNotification>().HasQueryFilter(cn =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            (
                cn.CompanyId == _currentUserContext.CompanyId &&
                (
                    _currentUserContext.IsCompanyAdmin ||
                    _currentUserContext.AccessScope == "CompanyLevel" ||
                    cn.UserId == _currentUserContext.UserId
                )
            )
        );

        // 17. Dealership Review Isolation:
        modelBuilder.Entity<DealershipReview>().HasQueryFilter(dr =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            dr.CompanyId == _currentUserContext.CompanyId
        );

        // 18. Payment Transaction Isolation:
        modelBuilder.Entity<PaymentTransaction>().HasQueryFilter(pt =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            (
                pt.CompanyId == _currentUserContext.CompanyId &&
                (
                    _currentUserContext.IsCompanyAdmin ||
                    _currentUserContext.AccessScope == "CompanyLevel" ||
                    pt.BuyerId == _currentUserContext.UserId ||
                    pt.BranchId == _currentUserContext.BranchId
                )
            )
        );

        // 19. Customer Document Isolation:
        modelBuilder.Entity<CustomerDocument>().HasQueryFilter(cd =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            (
                cd.CompanyId == _currentUserContext.CompanyId &&
                (
                    _currentUserContext.IsCompanyAdmin ||
                    _currentUserContext.AccessScope == "CompanyLevel" ||
                    _currentUserContext.Roles.Contains("BranchManager") ||
                    _currentUserContext.Roles.Contains("SalesExecutive") ||
                    cd.UserId == _currentUserContext.UserId
                )
            )
        );

        // 20. Warranty Package Isolation:
        modelBuilder.Entity<WarrantyPackage>().HasQueryFilter(wp =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            wp.CompanyId == _currentUserContext.CompanyId
        );

        // 21. Vehicle Warranty Subscription Isolation:
        modelBuilder.Entity<VehicleWarrantySubscription>().HasQueryFilter(ws =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            (
                ws.CompanyId == _currentUserContext.CompanyId &&
                (
                    _currentUserContext.IsCompanyAdmin ||
                    _currentUserContext.AccessScope == "CompanyLevel" ||
                    ws.BuyerId == _currentUserContext.UserId ||
                    ws.BranchId == _currentUserContext.BranchId
                )
            )
        );

        // 22. Sales Lead Isolation:
        modelBuilder.Entity<SalesLead>().HasQueryFilter(sl =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            (
                sl.CompanyId == _currentUserContext.CompanyId &&
                (
                    _currentUserContext.IsCompanyAdmin ||
                    _currentUserContext.AccessScope == "CompanyLevel" ||
                    sl.BranchId == _currentUserContext.BranchId ||
                    sl.AssignedStaffId == _currentUserContext.UserId
                )
            )
        );

        // 23. EV Charging Station Isolation:
        modelBuilder.Entity<EvChargingStation>().HasQueryFilter(ev =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            ev.CompanyId == _currentUserContext.CompanyId
        );

        // 24. Home Charger Installation Isolation:
        modelBuilder.Entity<HomeChargerInstallation>().HasQueryFilter(hci =>
            _currentUserContext == null ||
            !_currentUserContext.IsAuthenticated ||
            _currentUserContext.IsGlobalAdmin ||
            (
                hci.CompanyId == _currentUserContext.CompanyId &&
                (
                    _currentUserContext.IsCompanyAdmin ||
                    _currentUserContext.AccessScope == "CompanyLevel" ||
                    hci.BuyerId == _currentUserContext.UserId ||
                    hci.BranchId == _currentUserContext.BranchId
                )
            )
        );
    }
}
