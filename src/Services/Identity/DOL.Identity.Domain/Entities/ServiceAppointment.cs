using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class ServiceAppointment : AuditableEntity, IBranchScoped
{
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid BuyerId { get; private set; }

    public string VinNumber { get; private set; } = string.Empty;
    public string RegistrationNumber { get; private set; } = string.Empty;
    public Guid? VehicleVariantId { get; private set; }

    public ServiceType ServiceType { get; private set; } = ServiceType.PeriodicMaintenance;
    public DateTime AppointmentDate { get; private set; }
    public string TimeSlot { get; private set; } = "10:00 AM - 12:00 PM";
    public string? CustomerComments { get; private set; }

    public decimal EstimatedCost { get; private set; }
    public decimal? ActualCost { get; private set; }
    public string? WorkshopNotes { get; private set; }
    public ServiceAppointmentStatus Status { get; private set; } = ServiceAppointmentStatus.Scheduled;
    public DateTime? CompletedAt { get; private set; }

    public VehicleVariant? VehicleVariant { get; private set; }

    private ServiceAppointment() { } // EF Core

    public ServiceAppointment(
        Guid companyId,
        Guid branchId,
        Guid buyerId,
        string vinNumber,
        string registrationNumber,
        ServiceType serviceType,
        DateTime appointmentDate,
        string timeSlot,
        decimal estimatedCost,
        string? customerComments = null,
        Guid? vehicleVariantId = null)
    {
        CompanyId = companyId;
        BranchId = branchId;
        BuyerId = buyerId;
        VinNumber = vinNumber.Trim().ToUpperInvariant();
        RegistrationNumber = registrationNumber.Trim().ToUpperInvariant();
        ServiceType = serviceType;
        AppointmentDate = appointmentDate;
        TimeSlot = timeSlot.Trim();
        EstimatedCost = estimatedCost;
        CustomerComments = customerComments?.Trim();
        VehicleVariantId = vehicleVariantId;
        Status = ServiceAppointmentStatus.Scheduled;
    }

    public void StartService()
    {
        Status = ServiceAppointmentStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CompleteService(decimal actualCost, string? workshopNotes = null)
    {
        ActualCost = actualCost;
        WorkshopNotes = workshopNotes?.Trim();
        Status = ServiceAppointmentStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CancelService()
    {
        Status = ServiceAppointmentStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}
