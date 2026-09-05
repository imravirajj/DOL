using FluentValidation;

namespace DOL.Identity.Application.Commands.CreateScopedUser;

public class CreateScopedUserCommandValidator : AbstractValidator<CreateScopedUserCommand>
{
    public CreateScopedUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.");

        When(x => x.Scope == Domain.Enums.AccessScope.BranchLevel, () =>
        {
            RuleFor(x => x.BranchId)
                .NotEmpty().WithMessage("Branch ID is required for BranchLevel scope.");
        });
    }
}
