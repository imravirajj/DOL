using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class DeliveryInspection : AuditableEntity, IBranchScoped
{
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid VehicleStockId { get; private set; }
    public Guid InspectorStaffId { get; private set; }

    // 50-Point Checklist Essentials
    public int OdometerReadingKm { get; private set; }
    public int BatteryHealthPct { get; private set; } = 100;
    public bool ExteriorConditionOk { get; private set; } = true;
    public bool InteriorCleanOk { get; private set; } = true;
    public bool ToolKitAndSpareWheelOk { get; private set; } = true;
    public bool DocumentationOk { get; private set; } = true;

    public string? InspectionNotes { get; private set; }
    public bool IsCustomerAccepted { get; private set; } = false;
    public string? CustomerSignatureUrl { get; private set; }

    public VehicleOrder? Order { get; private set; }
    public VehicleStock? Stock { get; private set; }

    private DeliveryInspection() { } // EF Core

    public DeliveryInspection(
        Guid companyId,
        Guid branchId,
        Guid orderId,
        Guid vehicleStockId,
        Guid inspectorStaffId,
        int odometerReadingKm,
        int batteryHealthPct,
        bool exteriorConditionOk,
        bool interiorCleanOk,
        bool toolKitAndSpareWheelOk,
        bool documentationOk,
        string? inspectionNotes = null)
    {
        CompanyId = companyId;
        BranchId = branchId;
        OrderId = orderId;
        VehicleStockId = vehicleStockId;
        InspectorStaffId = inspectorStaffId;
        OdometerReadingKm = odometerReadingKm;
        BatteryHealthPct = batteryHealthPct;
        ExteriorConditionOk = exteriorConditionOk;
        InteriorCleanOk = interiorCleanOk;
        ToolKitAndSpareWheelOk = toolKitAndSpareWheelOk;
        DocumentationOk = documentationOk;
        InspectionNotes = inspectionNotes?.Trim();
        IsCustomerAccepted = false;
    }

    public void CustomerSignOff(string? signatureUrl)
    {
        IsCustomerAccepted = true;
        CustomerSignatureUrl = signatureUrl;
        UpdatedAt = DateTime.UtcNow;
    }
}
