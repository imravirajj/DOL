using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class HomeChargerInstallation : AuditableEntity, IBranchScoped
{
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid BuyerId { get; private set; }
    public Guid? OrderId { get; private set; }

    public string InstallationAddress { get; private set; } = string.Empty;
    public DateTime PreferredSurveyDate { get; private set; }
    public string ChargerModel { get; private set; } = "7.4 kW AC Fast Home Charger";
    public HomeChargerSurveyStatus SurveyStatus { get; private set; } = HomeChargerSurveyStatus.Requested;
    public string? TechnicianNotes { get; private set; }
    public DateTime? InstalledAt { get; private set; }

    public VehicleOrder? Order { get; private set; }
    public ApplicationUser? Buyer { get; private set; }

    private HomeChargerInstallation() { } // EF Core

    public HomeChargerInstallation(
        Guid companyId,
        Guid branchId,
        Guid buyerId,
        string installationAddress,
        DateTime preferredSurveyDate,
        string chargerModel = "7.4 kW AC Fast Home Charger",
        Guid? orderId = null)
    {
        CompanyId = companyId;
        BranchId = branchId;
        BuyerId = buyerId;
        InstallationAddress = installationAddress.Trim();
        PreferredSurveyDate = preferredSurveyDate;
        ChargerModel = chargerModel.Trim();
        OrderId = orderId;
        SurveyStatus = HomeChargerSurveyStatus.Requested;
    }

    public void UpdateSurvey(HomeChargerSurveyStatus status, string? notes = null)
    {
        SurveyStatus = status;
        TechnicianNotes = notes?.Trim();
        if (status == HomeChargerSurveyStatus.Installed)
        {
            InstalledAt = DateTime.UtcNow;
        }
        UpdatedAt = DateTime.UtcNow;
    }
}
