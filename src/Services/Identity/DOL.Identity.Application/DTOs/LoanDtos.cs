using DOL.Identity.Domain.Enums;

namespace DOL.Identity.Application.DTOs;

public record EmiCalculationResultDto(
    decimal PrincipalAmount,
    decimal AnnualInterestRate,
    int TenureInMonths,
    decimal MonthlyEmi,
    decimal TotalInterestPayable,
    decimal TotalAmountPayable);

public record LoanApplicationDto(
    Guid Id,
    Guid CompanyId,
    Guid BranchId,
    Guid BuyerId,
    Guid QuotationId,
    decimal RequiredLoanAmount,
    int TenureInMonths,
    decimal MonthlyIncome,
    string PanNumber,
    string EmploymentType,
    string? SelectedBankName,
    decimal? ApprovedLoanAmount,
    decimal? ApprovedInterestRate,
    decimal? MonthlyEmi,
    string? SanctionLetterNumber,
    LoanStatus Status,
    DateTime CreatedAt);
