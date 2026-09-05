using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class EvChargingStation : AuditableEntity, ITenantScoped
{
    public Guid CompanyId { get; private set; }
    public Guid? BranchId { get; private set; }

    public string StationName { get; private set; } = string.Empty;
    public string LocationAddress { get; private set; } = string.Empty;
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public string ConnectorType { get; private set; } = "CCS2"; // CCS2, Type 2, Bharat DC-001
    public int PowerKw { get; private set; } = 60;              // 30, 60, 120, 240 kW
    public decimal TariffPerKwh { get; private set; } = 18.5m;
    public bool IsAvailable { get; private set; } = true;

    public Branch? Branch { get; private set; }

    private EvChargingStation() { } // EF Core

    public EvChargingStation(
        Guid companyId,
        string stationName,
        string locationAddress,
        double latitude,
        double longitude,
        string connectorType = "CCS2",
        int powerKw = 60,
        decimal tariffPerKwh = 18.5m,
        Guid? branchId = null)
    {
        CompanyId = companyId;
        StationName = stationName.Trim();
        LocationAddress = locationAddress.Trim();
        Latitude = latitude;
        Longitude = longitude;
        ConnectorType = connectorType.Trim().ToUpperInvariant();
        PowerKw = powerKw;
        TariffPerKwh = tariffPerKwh;
        BranchId = branchId;
        IsAvailable = true;
    }

    public void UpdateStatus(bool isAvailable, decimal? newTariff = null)
    {
        IsAvailable = isAvailable;
        if (newTariff.HasValue) TariffPerKwh = newTariff.Value;
        UpdatedAt = DateTime.UtcNow;
    }
}
