using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class TestDriveBooking : AuditableEntity, IBranchScoped
{
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid BuyerId { get; private set; }
    public Guid VehicleVariantId { get; private set; }

    public string CustomerName { get; private set; } = string.Empty;
    public string CustomerPhone { get; private set; } = string.Empty;
    public string CustomerEmail { get; private set; } = string.Empty;
    public string DrivingLicenseNumber { get; private set; } = string.Empty;

    public DateTime ScheduledDate { get; private set; }
    public string TimeSlot { get; private set; } = "10:00 AM - 11:00 AM";
    public DeliveryType LocationType { get; private set; } = DeliveryType.ShowroomPickup; // Showroom vs Home visit
    public string? HomeAddress { get; private set; }

    public TestDriveStatus Status { get; private set; } = TestDriveStatus.Scheduled;
    public int? Rating { get; private set; } // 1 to 5
    public string? FeedbackNotes { get; private set; }

    public VehicleVariant? VehicleVariant { get; private set; }

    private TestDriveBooking() { } // EF Core

    public TestDriveBooking(
        Guid companyId,
        Guid branchId,
        Guid buyerId,
        Guid vehicleVariantId,
        string customerName,
        string customerPhone,
        string customerEmail,
        string drivingLicenseNumber,
        DateTime scheduledDate,
        string timeSlot,
        DeliveryType locationType = DeliveryType.ShowroomPickup,
        string? homeAddress = null)
    {
        CompanyId = companyId;
        BranchId = branchId;
        BuyerId = buyerId;
        VehicleVariantId = vehicleVariantId;
        CustomerName = customerName.Trim();
        CustomerPhone = customerPhone.Trim();
        CustomerEmail = customerEmail.Trim().ToLowerInvariant();
        DrivingLicenseNumber = drivingLicenseNumber.Trim().ToUpperInvariant();
        ScheduledDate = scheduledDate.ToUniversalTime();
        TimeSlot = timeSlot.Trim();
        LocationType = locationType;
        HomeAddress = homeAddress?.Trim();
        Status = TestDriveStatus.Scheduled;
    }

    public void Reschedule(DateTime newDate, string newTimeSlot)
    {
        ScheduledDate = newDate.ToUniversalTime();
        TimeSlot = newTimeSlot.Trim();
        Status = TestDriveStatus.Scheduled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete(int rating, string? feedbackNotes)
    {
        Status = TestDriveStatus.Completed;
        Rating = Math.Clamp(rating, 1, 5);
        FeedbackNotes = feedbackNotes?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatus(TestDriveStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }
}
