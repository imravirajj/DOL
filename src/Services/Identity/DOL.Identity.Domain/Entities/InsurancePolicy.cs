using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class InsurancePolicy : AuditableEntity, IBranchScoped
{
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid BuyerId { get; private set; }

    public string InsurerName { get; private set; } = string.Empty; // e.g. "HDFC ERGO", "ICICI Lombard", "Bajaj Allianz"
    public string PolicyNumber { get; private set; } = string.Empty;
    public string PlanType { get; private set; } = "Comprehensive Zero-Dep";
    public decimal PremiumAmount { get; private set; }
    public decimal IdvAmount { get; private set; } // Insured Declared Value
    public DateTime CoverageStartDate { get; private set; }
    public DateTime CoverageEndDate { get; private set; }
    public string? PolicyDocumentUrl { get; private set; }
    public InsurancePolicyStatus Status { get; private set; } = InsurancePolicyStatus.Draft;

    public VehicleOrder? Order { get; private set; }

    private InsurancePolicy() { } // EF Core

    public InsurancePolicy(
        Guid companyId,
        Guid branchId,
        Guid orderId,
        Guid buyerId,
        string insurerName,
        string planType,
        decimal premiumAmount,
        decimal idvAmount,
        DateTime coverageStartDate,
        DateTime coverageEndDate,
        string? policyNumber = null,
        string? policyDocumentUrl = null)
    {
        CompanyId = companyId;
        BranchId = branchId;
        OrderId = orderId;
        BuyerId = buyerId;
        InsurerName = insurerName.Trim();
        PlanType = planType.Trim();
        PremiumAmount = premiumAmount;
        IdvAmount = idvAmount;
        CoverageStartDate = coverageStartDate;
        CoverageEndDate = coverageEndDate;
        PolicyNumber = policyNumber?.Trim().ToUpperInvariant() ?? string.Empty;
        PolicyDocumentUrl = policyDocumentUrl?.Trim();
        Status = string.IsNullOrWhiteSpace(PolicyNumber) ? InsurancePolicyStatus.Draft : InsurancePolicyStatus.Active;
    }

    public void IssuePolicy(string policyNumber, string? policyDocumentUrl = null)
    {
        PolicyNumber = policyNumber.Trim().ToUpperInvariant();
        PolicyDocumentUrl = policyDocumentUrl?.Trim();
        Status = InsurancePolicyStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CancelPolicy()
    {
        Status = InsurancePolicyStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}
