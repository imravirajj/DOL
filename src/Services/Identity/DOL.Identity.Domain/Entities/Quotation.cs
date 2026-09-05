using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class Quotation : AuditableEntity, IBranchScoped
{
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid VehicleVariantId { get; private set; }
    public Guid? BuyerId { get; private set; }

    public string QuotationNumber { get; private set; } = string.Empty; // e.g. "QTN-2026-MUM-00101"
    public string CustomerName { get; private set; } = string.Empty;
    public string CustomerEmail { get; private set; } = string.Empty;
    public string CustomerPhone { get; private set; } = string.Empty;
    public string SelectedColor { get; private set; } = string.Empty;

    // Price Breakdown Components
    public decimal ExShowroomPrice { get; private set; }
    public decimal RtoTaxAmount { get; private set; }
    public decimal InsuranceBaseAmount { get; private set; }
    public decimal InsuranceAddonsAmount { get; private set; }
    public decimal FastagCharges { get; private set; } = 500m;
    public decimal TcsAmount { get; private set; }
    public decimal AccessoriesTotal { get; private set; }
    public decimal ExtendedWarrantyAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TotalOnRoadPrice { get; private set; }

    // Selected Add-on Options
    public bool IncludeZeroDep { get; private set; }
    public bool IncludeEngineProtect { get; private set; }
    public bool IncludeReturnToInvoice { get; private set; }
    public bool IncludeExtendedWarranty { get; private set; }
    public string? SelectedAccessoriesJson { get; private set; }

    public QuotationStatus Status { get; private set; } = QuotationStatus.Active;
    public DateTime ValidUntil { get; private set; }

    public Branch? Branch { get; private set; }
    public VehicleVariant? VehicleVariant { get; private set; }
    public ApplicationUser? Buyer { get; private set; }

    private Quotation() { } // EF Core

    public Quotation(
        Guid companyId,
        Guid branchId,
        Guid vehicleVariantId,
        Guid? buyerId,
        string quotationNumber,
        string customerName,
        string customerEmail,
        string customerPhone,
        string selectedColor,
        decimal exShowroomPrice,
        decimal rtoTaxAmount,
        decimal insuranceBaseAmount,
        decimal insuranceAddonsAmount,
        decimal fastagCharges,
        decimal tcsAmount,
        decimal accessoriesTotal,
        decimal extendedWarrantyAmount,
        decimal discountAmount,
        bool includeZeroDep,
        bool includeEngineProtect,
        bool includeReturnToInvoice,
        bool includeExtendedWarranty,
        string? selectedAccessoriesJson,
        TimeSpan validityDuration)
    {
        CompanyId = companyId;
        BranchId = branchId;
        VehicleVariantId = vehicleVariantId;
        BuyerId = buyerId;
        QuotationNumber = quotationNumber.Trim().ToUpperInvariant();
        CustomerName = customerName.Trim();
        CustomerEmail = customerEmail.Trim().ToLowerInvariant();
        CustomerPhone = customerPhone.Trim();
        SelectedColor = selectedColor.Trim();

        ExShowroomPrice = exShowroomPrice;
        RtoTaxAmount = rtoTaxAmount;
        InsuranceBaseAmount = insuranceBaseAmount;
        InsuranceAddonsAmount = insuranceAddonsAmount;
        FastagCharges = fastagCharges;
        TcsAmount = tcsAmount;
        AccessoriesTotal = accessoriesTotal;
        ExtendedWarrantyAmount = extendedWarrantyAmount;
        DiscountAmount = discountAmount;

        // Auto-calculate exact on-road price
        TotalOnRoadPrice = (exShowroomPrice + rtoTaxAmount + insuranceBaseAmount + insuranceAddonsAmount +
                           fastagCharges + tcsAmount + accessoriesTotal + extendedWarrantyAmount) - discountAmount;

        IncludeZeroDep = includeZeroDep;
        IncludeEngineProtect = includeEngineProtect;
        IncludeReturnToInvoice = includeReturnToInvoice;
        IncludeExtendedWarranty = includeExtendedWarranty;
        SelectedAccessoriesJson = selectedAccessoriesJson;

        Status = QuotationStatus.Active;
        ValidUntil = DateTime.UtcNow.Add(validityDuration);
    }

    public void MarkConvertedToBooking()
    {
        Status = QuotationStatus.ConvertedToBooking;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Expire()
    {
        if (Status == QuotationStatus.Active && DateTime.UtcNow > ValidUntil)
        {
            Status = QuotationStatus.Expired;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
