using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class LoanApplication : AuditableEntity, IBranchScoped
{
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid BuyerId { get; private set; }
    public Guid QuotationId { get; private set; }

    public decimal RequiredLoanAmount { get; private set; }
    public int TenureInMonths { get; private set; }
    public decimal MonthlyIncome { get; private set; }
    public string PanNumber { get; private set; } = string.Empty;
    public string EmploymentType { get; private set; } = "Salaried"; // Salaried, Self-Employed, Business

    public string? SelectedBankName { get; private set; }
    public decimal? ApprovedLoanAmount { get; private set; }
    public decimal? ApprovedInterestRate { get; private set; }
    public decimal? MonthlyEmi { get; private set; }
    public string? SanctionLetterNumber { get; private set; }

    public LoanStatus Status { get; private set; } = LoanStatus.Applied;

    public Quotation? Quotation { get; private set; }

    private LoanApplication() { } // EF Core

    public LoanApplication(
        Guid companyId,
        Guid branchId,
        Guid buyerId,
        Guid quotationId,
        decimal requiredLoanAmount,
        int tenureInMonths,
        decimal monthlyIncome,
        string panNumber,
        string employmentType)
    {
        CompanyId = companyId;
        BranchId = branchId;
        BuyerId = buyerId;
        QuotationId = quotationId;
        RequiredLoanAmount = requiredLoanAmount;
        TenureInMonths = tenureInMonths;
        MonthlyIncome = monthlyIncome;
        PanNumber = panNumber.Trim().ToUpperInvariant();
        EmploymentType = employmentType.Trim();
        Status = LoanStatus.Applied;
    }

    public void Sanction(
        string bankName,
        decimal approvedAmount,
        decimal interestRate,
        decimal monthlyEmi,
        string sanctionLetterNumber)
    {
        SelectedBankName = bankName.Trim();
        ApprovedLoanAmount = approvedAmount;
        ApprovedInterestRate = interestRate;
        MonthlyEmi = monthlyEmi;
        SanctionLetterNumber = sanctionLetterNumber.Trim();
        Status = LoanStatus.Sanctioned;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(LoanStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateFinancials(decimal requiredLoanAmount, int tenureInMonths, decimal monthlyIncome)
    {
        RequiredLoanAmount = requiredLoanAmount;
        TenureInMonths = tenureInMonths;
        MonthlyIncome = monthlyIncome;
        UpdatedAt = DateTime.UtcNow;
    }
}
