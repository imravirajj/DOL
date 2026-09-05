using DOL.Identity.Application.DTOs;
using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Queries.Companies;

public record GetCompaniesQuery(
    CompanyStatus? Status = null,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<Result<List<CompanyDto>>>;

public class GetCompaniesQueryHandler : IRequestHandler<GetCompaniesQuery, Result<List<CompanyDto>>>
{
    private readonly IIdentityDbContext _context;

    public GetCompaniesQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<CompanyDto>>> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Companies
            .Include(c => c.Branches)
            .AsNoTracking();

        if (request.Status.HasValue)
        {
            query = query.Where(c => c.Status == request.Status.Value);
        }

        var companies = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CompanyDto(
                c.Id,
                c.Name,
                c.Code,
                c.Email,
                c.PhoneNumber,
                c.Address,
                c.SubscriptionPlan,
                c.Status.ToString(),
                c.Currency,
                c.TimeZone,
                c.CreatedAt,
                c.Branches.Count))
            .ToListAsync(cancellationToken);

        return Result<List<CompanyDto>>.Success(companies);
    }
}

public record GetCompanyByIdQuery(Guid Id) : IRequest<Result<CompanyDto>>;

public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, Result<CompanyDto>>
{
    private readonly IIdentityDbContext _context;

    public GetCompanyByIdQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CompanyDto>> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies
            .Include(c => c.Branches)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (company == null)
        {
            return Result<CompanyDto>.Failure("Company not found.");
        }

        var dto = new CompanyDto(
            company.Id,
            company.Name,
            company.Code,
            company.Email,
            company.PhoneNumber,
            company.Address,
            company.SubscriptionPlan,
            company.Status.ToString(),
            company.Currency,
            company.TimeZone,
            company.CreatedAt,
            company.Branches.Count);

        return Result<CompanyDto>.Success(dto);
    }
}
