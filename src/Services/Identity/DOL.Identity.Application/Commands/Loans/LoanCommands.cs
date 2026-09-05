using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Loans;

// ── EMI Calculator Query ────────────────────────────────────
public record CalculateEmiQuery(
    decimal PrincipalAmount,
    decimal AnnualInterestRate,
    int TenureInMonths) : IRequest<Result<EmiCalculationResultDto>>;

public class CalculateEmiQueryHandler : IRequestHandler<CalculateEmiQuery, Result<EmiCalculationResultDto>>
{
    public Task<Result<EmiCalculationResultDto>> Handle(CalculateEmiQuery request, CancellationToken cancellationToken)
    {
        if (request.PrincipalAmount <= 0 || request.TenureInMonths <= 0 || request.AnnualInterestRate <= 0)
        {
            return Task.FromResult(Result<EmiCalculationResultDto>.Failure("Invalid input values for EMI calculation."));
        }

        // Standard Equated Monthly Installment Formula: E = P * r * (1+r)^n / ((1+r)^n - 1)
        double p = (double)request.PrincipalAmount;
        double r = (double)(request.AnnualInterestRate / (12 * 100)); // monthly interest rate
        int n = request.TenureInMonths;

        double emi = (p * r * Math.Pow(1 + r, n)) / (Math.Pow(1 + r, n) - 1);
        decimal monthlyEmi = Math.Round((decimal)emi, 2);
        decimal totalPayable = Math.Round(monthlyEmi * n, 2);
        decimal totalInterest = Math.Round(totalPayable - request.PrincipalAmount, 2);

        var result = new EmiCalculationResultDto(
            request.PrincipalAmount,
            request.AnnualInterestRate,
            request.TenureInMonths,
            monthlyEmi,
            totalInterest,
            totalPayable);

        return Task.FromResult(Result<EmiCalculationResultDto>.Success(result));
    }
}

// ── Apply for Loan ──────────────────────────────────────────
public record ApplyLoanCommand(
    Guid CompanyId,
    Guid BranchId,
    Guid BuyerId,
    Guid QuotationId,
    decimal RequiredLoanAmount,
    int TenureInMonths,
    decimal MonthlyIncome,
    string PanNumber,
    string EmploymentType) : IRequest<Result<Guid>>;

public class ApplyLoanCommandValidator : AbstractValidator<ApplyLoanCommand>
{
    public ApplyLoanCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.BuyerId).NotEmpty();
        RuleFor(x => x.QuotationId).NotEmpty();
        RuleFor(x => x.RequiredLoanAmount).GreaterThan(0);
        RuleFor(x => x.TenureInMonths).InclusiveBetween(6, 84);
        RuleFor(x => x.MonthlyIncome).GreaterThan(0);
        RuleFor(x => x.PanNumber).NotEmpty().Length(10).WithMessage("PAN must be exactly 10 characters.");
    }
}

public class ApplyLoanCommandHandler : IRequestHandler<ApplyLoanCommand, Result<Guid>>
{
    private readonly IIdentityDbContext _context;

    public ApplyLoanCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(ApplyLoanCommand request, CancellationToken cancellationToken)
    {
        var quotation = await _context.Quotations.FirstOrDefaultAsync(q => q.Id == request.QuotationId, cancellationToken);
        if (quotation == null) return Result<Guid>.Failure("Quotation not found.");

        var loan = new LoanApplication(
            request.CompanyId,
            request.BranchId,
            request.BuyerId,
            request.QuotationId,
            request.RequiredLoanAmount,
            request.TenureInMonths,
            request.MonthlyIncome,
            request.PanNumber,
            request.EmploymentType);

        _context.LoanApplications.Add(loan);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(loan.Id);
    }
}

// ── Sanction Loan ───────────────────────────────────────────
public record SanctionLoanCommand(
    Guid Id,
    string BankName,
    decimal ApprovedAmount,
    decimal InterestRate,
    decimal MonthlyEmi,
    string SanctionLetterNumber) : IRequest<Result>;

public class SanctionLoanCommandHandler : IRequestHandler<SanctionLoanCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public SanctionLoanCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(SanctionLoanCommand request, CancellationToken cancellationToken)
    {
        var loan = await _context.LoanApplications.FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);
        if (loan == null) return Result.Failure("Loan application not found.");

        loan.Sanction(request.BankName, request.ApprovedAmount, request.InterestRate, request.MonthlyEmi, request.SanctionLetterNumber);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

// ── Update Loan Financials ──────────────────────────────────
public record UpdateLoanCommand(
    Guid Id,
    decimal RequiredLoanAmount,
    int TenureInMonths,
    decimal MonthlyIncome) : IRequest<Result>;

public class UpdateLoanCommandHandler : IRequestHandler<UpdateLoanCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public UpdateLoanCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateLoanCommand request, CancellationToken cancellationToken)
    {
        var loan = await _context.LoanApplications.FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);
        if (loan == null) return Result.Failure("Loan application not found.");

        loan.UpdateFinancials(request.RequiredLoanAmount, request.TenureInMonths, request.MonthlyIncome);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

// ── Delete / Reject Loan ────────────────────────────────────
public record DeleteLoanCommand(Guid Id) : IRequest<Result>;

public class DeleteLoanCommandHandler : IRequestHandler<DeleteLoanCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public DeleteLoanCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteLoanCommand request, CancellationToken cancellationToken)
    {
        var loan = await _context.LoanApplications.FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);
        if (loan == null) return Result.Failure("Loan application not found.");

        loan.UpdateStatus(LoanStatus.Rejected);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

// ── Get Loans Query ─────────────────────────────────────────
public record GetLoansQuery(
    Guid? BuyerId = null,
    LoanStatus? Status = null) : IRequest<Result<List<LoanApplicationDto>>>;

public class GetLoansQueryHandler : IRequestHandler<GetLoansQuery, Result<List<LoanApplicationDto>>>
{
    private readonly IIdentityDbContext _context;

    public GetLoansQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<LoanApplicationDto>>> Handle(GetLoansQuery request, CancellationToken cancellationToken)
    {
        var query = _context.LoanApplications.AsNoTracking();

        if (request.BuyerId.HasValue) query = query.Where(l => l.BuyerId == request.BuyerId.Value);
        if (request.Status.HasValue) query = query.Where(l => l.Status == request.Status.Value);

        var list = await query
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new LoanApplicationDto(
                l.Id,
                l.CompanyId,
                l.BranchId,
                l.BuyerId,
                l.QuotationId,
                l.RequiredLoanAmount,
                l.TenureInMonths,
                l.MonthlyIncome,
                l.PanNumber,
                l.EmploymentType,
                l.SelectedBankName,
                l.ApprovedLoanAmount,
                l.ApprovedInterestRate,
                l.MonthlyEmi,
                l.SanctionLetterNumber,
                l.Status,
                l.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<LoanApplicationDto>>.Success(list);
    }
}

public record GetLoanByIdQuery(Guid Id) : IRequest<Result<LoanApplicationDto>>;

public class GetLoanByIdQueryHandler : IRequestHandler<GetLoanByIdQuery, Result<LoanApplicationDto>>
{
    private readonly IIdentityDbContext _context;

    public GetLoanByIdQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<LoanApplicationDto>> Handle(GetLoanByIdQuery request, CancellationToken cancellationToken)
    {
        var l = await _context.LoanApplications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (l == null) return Result<LoanApplicationDto>.Failure("Loan application not found.");

        var dto = new LoanApplicationDto(
            l.Id,
            l.CompanyId,
            l.BranchId,
            l.BuyerId,
            l.QuotationId,
            l.RequiredLoanAmount,
            l.TenureInMonths,
            l.MonthlyIncome,
            l.PanNumber,
            l.EmploymentType,
            l.SelectedBankName,
            l.ApprovedLoanAmount,
            l.ApprovedInterestRate,
            l.MonthlyEmi,
            l.SanctionLetterNumber,
            l.Status,
            l.CreatedAt);

        return Result<LoanApplicationDto>.Success(dto);
    }
}
