using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class VehicleTradeIn : AuditableEntity, IBranchScoped
{
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid BuyerId { get; private set; }

    public string Make { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public int KilometersDriven { get; private set; }
    public string FuelType { get; private set; } = "Petrol";
    public string Condition { get; private set; } = "Good"; // Excellent, Good, Fair, Poor
    public bool HasAccidentHistory { get; private set; }
    public string? RegistrationNumber { get; private set; }

    public decimal EstimatedValue { get; private set; }
    public decimal? OfferedValue { get; private set; }
    public DateTime? InspectionDate { get; private set; }
    public string? InspectorNotes { get; private set; }
    public TradeInStatus Status { get; private set; } = TradeInStatus.Valuated;

    private VehicleTradeIn() { } // EF Core

    public VehicleTradeIn(
        Guid companyId,
        Guid branchId,
        Guid buyerId,
        string make,
        string model,
        int year,
        int kilometersDriven,
        string fuelType,
        string condition,
        bool hasAccidentHistory,
        decimal estimatedValue,
        string? registrationNumber = null)
    {
        CompanyId = companyId;
        BranchId = branchId;
        BuyerId = buyerId;
        Make = make.Trim();
        Model = model.Trim();
        Year = year;
        KilometersDriven = kilometersDriven;
        FuelType = fuelType.Trim();
        Condition = condition.Trim();
        HasAccidentHistory = hasAccidentHistory;
        EstimatedValue = estimatedValue;
        RegistrationNumber = registrationNumber?.Trim().ToUpperInvariant();
        Status = TradeInStatus.Valuated;
    }

    public void ScheduleInspection(DateTime inspectionDate)
    {
        InspectionDate = inspectionDate;
        Status = TradeInStatus.InspectionScheduled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ProvideOffer(decimal offeredValue, string? inspectorNotes = null)
    {
        OfferedValue = offeredValue;
        InspectorNotes = inspectorNotes?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void AcceptOffer()
    {
        Status = TradeInStatus.OfferAccepted;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RejectOffer()
    {
        Status = TradeInStatus.OfferRejected;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CompleteTradeIn()
    {
        Status = TradeInStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }
}
