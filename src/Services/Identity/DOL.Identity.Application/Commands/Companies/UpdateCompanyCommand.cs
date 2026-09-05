using DOL.Identity.Application.Interfaces;
using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Companies;

public record UpdateCompanyCommand(
    Guid Id,
    string Name,
    string PhoneNumber,
    string? Address,
    string Currency,
    string TimeZone) : IRequest<Result>;

public class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Company ID is required.");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Currency).NotEmpty().MaximumLength(10);
        RuleFor(x => x.TimeZone).NotEmpty().MaximumLength(50);
    }
}

public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public UpdateCompanyCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (company == null)
        {
            return Result.Failure("Company not found.");
        }

        company.UpdateDetails(request.Name, request.PhoneNumber, request.Address, request.Currency, request.TimeZone);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public record DeleteCompanyCommand(Guid Id) : IRequest<Result>;

public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public DeleteCompanyCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (company == null)
        {
            return Result.Failure("Company not found.");
        }

        // Soft delete / suspend company
        company.SetStatus(CompanyStatus.Suspended);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
