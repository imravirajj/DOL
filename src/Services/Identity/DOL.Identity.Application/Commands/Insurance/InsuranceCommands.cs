using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Insurance;

// ── Compare Insurance Plans ─────────────────────────────────
public record GetInsurancePlansQuery(decimal ExShowroomPrice) : IRequest<Result<List<InsurancePlanDto>>>;

public class GetInsurancePlansQueryHandler : IRequestHandler<GetInsurancePlansQuery, Result<List<InsurancePlanDto>>>
{
    public Task<Result<List<InsurancePlanDto>>> Handle(GetInsurancePlansQuery request, CancellationToken cancellationToken)
    {
        // 3.2% approx comprehensive premium benchmark
        decimal price = request.ExShowroomPrice > 0 ? request.ExShowroomPrice : 1_000_000m;
        decimal baseRate = price * 0.032m;

        var plans = new List<InsurancePlanDto>
        {
            new("HDFC ERGO General Insurance", "Titanium Zero-Dep Shield", Math.Round(baseRate * 1.05m, 0), 8500, true, true, true),
            new("ICICI Lombard", "Auto Secure Bumper-to-Bumper", Math.Round(baseRate * 0.98m, 0), 9100, true, true, true),
            new("Bajaj Allianz", "Drive Smart Comprehensive", Math.Round(baseRate * 0.92m, 0), 7800, true, false, true),
            new("Tata AIG General Insurance", "Auto Protect Gold", Math.Round(baseRate * 1.02m, 0), 8200, true, true, false)
        };

        return Task.FromResult(Result<List<InsurancePlanDto>>.Success(plans));
    }
}

// ── Issue Insurance Policy ──────────────────────────────────
public record IssueInsurancePolicyCommand(
    Guid CompanyId,
    Guid BranchId,
    Guid OrderId,
    Guid BuyerId,
    string InsurerName,
    string PlanType,
    decimal PremiumAmount,
    decimal IdvAmount,
    DateTime CoverageStartDate,
    DateTime CoverageEndDate,
    string PolicyNumber,
    string? PolicyDocumentUrl = null) : IRequest<Result<Guid>>;

public class IssueInsurancePolicyCommandValidator : AbstractValidator<IssueInsurancePolicyCommand>
{
    public IssueInsurancePolicyCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.BuyerId).NotEmpty();
        RuleFor(x => x.InsurerName).NotEmpty();
        RuleFor(x => x.PolicyNumber).NotEmpty();
        RuleFor(x => x.PremiumAmount).GreaterThan(0);
        RuleFor(x => x.CoverageEndDate).GreaterThan(x => x.CoverageStartDate);
    }
}

public class IssueInsurancePolicyCommandHandler : IRequestHandler<IssueInsurancePolicyCommand, Result<Guid>>
{
    private readonly IIdentityDbContext _context;

    public IssueInsurancePolicyCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(IssueInsurancePolicyCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.VehicleOrders.FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);
        if (order == null) return Result<Guid>.Failure("Order not found.");

        var existingPolicy = await _context.InsurancePolicies
            .FirstOrDefaultAsync(p => p.OrderId == request.OrderId && p.Status == InsurancePolicyStatus.Active, cancellationToken);
        if (existingPolicy != null) return Result<Guid>.Failure("An active insurance policy is already issued for this order.");

        var policy = new InsurancePolicy(
            request.CompanyId,
            request.BranchId,
            request.OrderId,
            request.BuyerId,
            request.InsurerName,
            request.PlanType,
            request.PremiumAmount,
            request.IdvAmount,
            request.CoverageStartDate,
            request.CoverageEndDate,
            request.PolicyNumber,
            request.PolicyDocumentUrl);

        _context.InsurancePolicies.Add(policy);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(policy.Id);
    }
}

// ── Get Policy By Order ─────────────────────────────────────
public record GetInsurancePolicyByOrderQuery(Guid OrderId) : IRequest<Result<InsurancePolicyDto>>;

public class GetInsurancePolicyByOrderQueryHandler : IRequestHandler<GetInsurancePolicyByOrderQuery, Result<InsurancePolicyDto>>
{
    private readonly IIdentityDbContext _context;

    public GetInsurancePolicyByOrderQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<InsurancePolicyDto>> Handle(GetInsurancePolicyByOrderQuery request, CancellationToken cancellationToken)
    {
        var p = await _context.InsurancePolicies.AsNoTracking()
            .FirstOrDefaultAsync(x => x.OrderId == request.OrderId, cancellationToken);

        if (p == null) return Result<InsurancePolicyDto>.Failure("No insurance policy found for this order.");

        return Result<InsurancePolicyDto>.Success(new InsurancePolicyDto(
            p.Id,
            p.CompanyId,
            p.BranchId,
            p.BuyerId,
            p.OrderId,
            p.InsurerName,
            p.PolicyNumber,
            p.PlanType,
            p.PremiumAmount,
            p.IdvAmount,
            p.CoverageStartDate,
            p.CoverageEndDate,
            p.PolicyDocumentUrl,
            p.Status,
            p.CreatedAt));
    }
}

// ── Cancel Policy ───────────────────────────────────────────
public record CancelInsurancePolicyCommand(Guid Id) : IRequest<Result<bool>>;

public class CancelInsurancePolicyCommandHandler : IRequestHandler<CancelInsurancePolicyCommand, Result<bool>>
{
    private readonly IIdentityDbContext _context;

    public CancelInsurancePolicyCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(CancelInsurancePolicyCommand request, CancellationToken cancellationToken)
    {
        var policy = await _context.InsurancePolicies.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (policy == null) return Result<bool>.Failure("Insurance policy not found.");

        policy.CancelPolicy();
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
