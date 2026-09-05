using FluentValidation;

namespace DOL.Identity.Application.Commands.CreateBranch;

public class CreateBranchCommandValidator : AbstractValidator<CreateBranchCommand>
{
    public CreateBranchCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Branch name is required.")
            .MaximumLength(150).WithMessage("Branch name cannot exceed 150 characters.");

        RuleFor(x => x.BranchCode)
            .NotEmpty().WithMessage("Branch code is required.")
            .MaximumLength(50).WithMessage("Branch code cannot exceed 50 characters.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MaximumLength(500).WithMessage("Address cannot exceed 500 characters.");

        RuleFor(x => x.CityId)
            .NotEmpty().WithMessage("City ID is required.");
    }
}
