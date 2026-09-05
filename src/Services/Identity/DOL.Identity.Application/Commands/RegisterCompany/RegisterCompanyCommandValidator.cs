using FluentValidation;

namespace DOL.Identity.Application.Commands.RegisterCompany;

public class RegisterCompanyCommandValidator : AbstractValidator<RegisterCompanyCommand>
{
    public RegisterCompanyCommandValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(200).WithMessage("Company name cannot exceed 200 characters.");

        RuleFor(x => x.CompanyCode)
            .NotEmpty().WithMessage("Company code is required.")
            .MaximumLength(50).WithMessage("Company code cannot exceed 50 characters.");

        RuleFor(x => x.CompanyEmail)
            .NotEmpty().WithMessage("Company email is required.")
            .EmailAddress().WithMessage("Invalid company email format.");

        RuleFor(x => x.CompanyPhone)
            .NotEmpty().WithMessage("Company phone is required.");

        RuleFor(x => x.AdminFirstName)
            .NotEmpty().WithMessage("Admin first name is required.");

        RuleFor(x => x.AdminLastName)
            .NotEmpty().WithMessage("Admin last name is required.");

        RuleFor(x => x.AdminEmail)
            .NotEmpty().WithMessage("Admin email is required.")
            .EmailAddress().WithMessage("Invalid admin email format.");

        RuleFor(x => x.AdminPassword)
            .NotEmpty().WithMessage("Admin password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
    }
}
