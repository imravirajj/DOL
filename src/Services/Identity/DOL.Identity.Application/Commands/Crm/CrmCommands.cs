using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Crm;

// ── Create Sales Lead ───────────────────────────────────────
public record CreateLeadCommand(
    Guid CompanyId,
    Guid BranchId,
    string CustomerName,
    string CustomerPhone,
    string? CustomerEmail = null,
    string LeadSource = "Website",
    LeadPriority Priority = LeadPriority.Hot,
    Guid? InterestedModelId = null,
    Guid? AssignedStaffId = null,
    string? Notes = null,
    DateTime? NextFollowUpDate = null) : IRequest<Result<Guid>>;

public class CreateLeadCommandValidator : AbstractValidator<CreateLeadCommand>
{
    public CreateLeadCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.CustomerName).NotEmpty();
        RuleFor(x => x.CustomerPhone).NotEmpty();
    }
}

public class CreateLeadCommandHandler : IRequestHandler<CreateLeadCommand, Result<Guid>>
{
    private readonly IIdentityDbContext _context;

    public CreateLeadCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateLeadCommand request, CancellationToken cancellationToken)
    {
        var lead = new SalesLead(
            request.CompanyId,
            request.BranchId,
            request.CustomerName,
            request.CustomerPhone,
            request.CustomerEmail,
            request.LeadSource,
            request.Priority,
            request.InterestedModelId,
            request.AssignedStaffId,
            request.Notes,
            request.NextFollowUpDate);

        _context.SalesLeads.Add(lead);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(lead.Id);
    }
}

// ── Assign Lead ─────────────────────────────────────────────
public record AssignLeadCommand(Guid LeadId, Guid StaffId) : IRequest<Result<bool>>;

public class AssignLeadCommandHandler : IRequestHandler<AssignLeadCommand, Result<bool>>
{
    private readonly IIdentityDbContext _context;

    public AssignLeadCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(AssignLeadCommand request, CancellationToken cancellationToken)
    {
        var lead = await _context.SalesLeads.FirstOrDefaultAsync(l => l.Id == request.LeadId, cancellationToken);
        if (lead == null) return Result<bool>.Failure("Lead not found.");

        lead.AssignStaff(request.StaffId);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}

// ── Update Lead Stage ───────────────────────────────────────
public record UpdateLeadStageCommand(Guid LeadId, LeadStage Stage, string? LostReason = null) : IRequest<Result<bool>>;

public class UpdateLeadStageCommandHandler : IRequestHandler<UpdateLeadStageCommand, Result<bool>>
{
    private readonly IIdentityDbContext _context;

    public UpdateLeadStageCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateLeadStageCommand request, CancellationToken cancellationToken)
    {
        var lead = await _context.SalesLeads.FirstOrDefaultAsync(l => l.Id == request.LeadId, cancellationToken);
        if (lead == null) return Result<bool>.Failure("Lead not found.");

        lead.AdvanceStage(request.Stage, request.LostReason);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}

// ── Schedule Follow-Up ──────────────────────────────────────
public record ScheduleFollowUpCommand(Guid LeadId, DateTime NextFollowUpDate, string? Notes = null) : IRequest<Result<bool>>;

public class ScheduleFollowUpCommandHandler : IRequestHandler<ScheduleFollowUpCommand, Result<bool>>
{
    private readonly IIdentityDbContext _context;

    public ScheduleFollowUpCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(ScheduleFollowUpCommand request, CancellationToken cancellationToken)
    {
        var lead = await _context.SalesLeads.FirstOrDefaultAsync(l => l.Id == request.LeadId, cancellationToken);
        if (lead == null) return Result<bool>.Failure("Lead not found.");

        lead.ScheduleFollowUp(request.NextFollowUpDate, request.Notes);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}

// ── Delete / Archive Lead ───────────────────────────────────
public record DeleteLeadCommand(Guid Id) : IRequest<Result<bool>>;

public class DeleteLeadCommandHandler : IRequestHandler<DeleteLeadCommand, Result<bool>>
{
    private readonly IIdentityDbContext _context;

    public DeleteLeadCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteLeadCommand request, CancellationToken cancellationToken)
    {
        var lead = await _context.SalesLeads.FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);
        if (lead == null) return Result<bool>.Failure("Lead not found.");

        _context.SalesLeads.Remove(lead);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}

// ── Queries ─────────────────────────────────────────────────
public record GetLeadsQuery(Guid? AssignedStaffId = null, LeadStage? Stage = null) : IRequest<Result<List<SalesLeadDto>>>;

public class GetLeadsQueryHandler : IRequestHandler<GetLeadsQuery, Result<List<SalesLeadDto>>>
{
    private readonly IIdentityDbContext _context;

    public GetLeadsQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<SalesLeadDto>>> Handle(GetLeadsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.SalesLeads.AsNoTracking();

        if (request.AssignedStaffId.HasValue)
        {
            query = query.Where(l => l.AssignedStaffId == request.AssignedStaffId.Value);
        }

        if (request.Stage.HasValue)
        {
            query = query.Where(l => l.Stage == request.Stage.Value);
        }

        var list = await query
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new SalesLeadDto(
                l.Id,
                l.CompanyId,
                l.BranchId,
                l.AssignedStaffId,
                l.InterestedModelId,
                l.CustomerName,
                l.CustomerPhone,
                l.CustomerEmail,
                l.LeadSource,
                l.Priority,
                l.Stage,
                l.Notes,
                l.NextFollowUpDate,
                l.LostReason,
                l.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<SalesLeadDto>>.Success(list);
    }
}

public record GetLeadByIdQuery(Guid Id) : IRequest<Result<SalesLeadDto>>;

public class GetLeadByIdQueryHandler : IRequestHandler<GetLeadByIdQuery, Result<SalesLeadDto>>
{
    private readonly IIdentityDbContext _context;

    public GetLeadByIdQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SalesLeadDto>> Handle(GetLeadByIdQuery request, CancellationToken cancellationToken)
    {
        var l = await _context.SalesLeads.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (l == null) return Result<SalesLeadDto>.Failure("Lead not found.");

        return Result<SalesLeadDto>.Success(new SalesLeadDto(
            l.Id,
            l.CompanyId,
            l.BranchId,
            l.AssignedStaffId,
            l.InterestedModelId,
            l.CustomerName,
            l.CustomerPhone,
            l.CustomerEmail,
            l.LeadSource,
            l.Priority,
            l.Stage,
            l.Notes,
            l.NextFollowUpDate,
            l.LostReason,
            l.CreatedAt));
    }
}
