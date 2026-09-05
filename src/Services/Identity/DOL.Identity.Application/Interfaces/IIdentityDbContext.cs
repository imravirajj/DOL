using DOL.Identity.Domain.Entities;
using DOL.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Interfaces;

public interface IIdentityDbContext : IUnitOfWork
{
    DbSet<ApplicationUser> Users { get; }
    DbSet<ApplicationRole> Roles { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Company> Companies { get; }
    DbSet<Country> Countries { get; }
    DbSet<StateRegion> StateRegions { get; }
    DbSet<City> Cities { get; }
    DbSet<Branch> Branches { get; }
    DbSet<VehicleModel> VehicleModels { get; }
    DbSet<VehicleVariant> VehicleVariants { get; }
    DbSet<VehicleStock> VehicleStocks { get; }
    DbSet<WaitlistEntry> WaitlistEntries { get; }
    DbSet<Quotation> Quotations { get; }
    DbSet<RtoTaxSlab> RtoTaxSlabs { get; }
    DbSet<LoanApplication> LoanApplications { get; }
    DbSet<VehicleOrder> VehicleOrders { get; }
    DbSet<TestDriveBooking> TestDriveBookings { get; }
    DbSet<DeliveryInspection> DeliveryInspections { get; }
    DbSet<VehicleTradeIn> VehicleTradeIns { get; }
    DbSet<VehicleAccessory> VehicleAccessories { get; }
    DbSet<InsurancePolicy> InsurancePolicies { get; }
    DbSet<ServiceAppointment> ServiceAppointments { get; }
    DbSet<CustomerNotification> CustomerNotifications { get; }
    DbSet<DealershipReview> DealershipReviews { get; }
}

