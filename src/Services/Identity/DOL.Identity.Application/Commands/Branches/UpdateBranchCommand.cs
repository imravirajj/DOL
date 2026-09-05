using DOL.Identity.Application.Interfaces;
using DOL.SharedKernel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DOL.Identity.Application.Commands.Branches;

public record UpdateBranchCommand(
    Guid Id,
    string Name,
    string Address,
    string? ContactPhone,
    string? ContactEmail) : IRequest<Result>;

public class UpdateBranchCommandValidator : AbstractValidator<UpdateBranchCommand>
{
    public UpdateBranchCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Branch ID is required.");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(300);
        RuleFor(x => x.ContactEmail).EmailAddress().When(x => !string.IsNullOrEmpty(x.ContactEmail));
    }
}

public class UpdateBranchCommandHandler : IRequestHandler<UpdateBranchCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public UpdateBranchCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
        if (branch == null)
        {
            return Result.Failure("Branch not found.");
        }

        branch.UpdateDetails(request.Name, request.Address, request.ContactPhone, request.ContactEmail);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public record DeleteBranchCommand(Guid Id) : IRequest<Result>;

public class DeleteBranchCommandHandler : IRequestHandler<DeleteBranchCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public DeleteBranchCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
        if (branch == null)
        {
            return Result.Failure("Branch not found.");
        }

        branch.SetActiveStatus(false);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
