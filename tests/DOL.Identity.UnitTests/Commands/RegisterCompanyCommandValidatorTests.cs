using DOL.Identity.Application.Commands.RegisterCompany;
using FluentAssertions;
using Xunit;

namespace DOL.Identity.UnitTests.Commands;

public class RegisterCompanyCommandValidatorTests
{
    private readonly RegisterCompanyCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCompanyRegistration_ShouldPass()
    {
        var command = new RegisterCompanyCommand(
            "Global Auto Corp",
            "GAC-001",
            "contact@globalauto.com",
            "+919876543210",
            "123 Corporate Park",
            "Rajesh",
            "Kumar",
            "rajesh@globalauto.com",
            "StrongAdmin@2026",
            "+919876543211"
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_MissingCompanyDetails_ShouldFail()
    {
        var command = new RegisterCompanyCommand(
            "",
            "",
            "invalid-email",
            "",
            null,
            "",
            "",
            "",
            "weak",
            ""
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CompanyName");
        result.Errors.Should().Contain(e => e.PropertyName == "CompanyCode");
        result.Errors.Should().Contain(e => e.PropertyName == "CompanyEmail");
    }
}
