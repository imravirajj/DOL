using DOL.Identity.Application.Commands.Register;
using FluentAssertions;
using Xunit;

namespace DOL.Identity.UnitTests.Commands;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new RegisterCommand(
            "Jane",
            "Smith",
            "jane.smith@example.com",
            "+919876543210",
            "StrongP@ss123",
            "Buyer"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_InvalidEmail_ShouldHaveValidationError()
    {
        // Arrange
        var command = new RegisterCommand(
            "Jane",
            "Smith",
            "invalid-email",
            "+919876543210",
            "StrongP@ss123",
            "Buyer"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validate_WeakPassword_ShouldHaveValidationError()
    {
        // Arrange
        var command = new RegisterCommand(
            "Jane",
            "Smith",
            "jane@example.com",
            "+919876543210",
            "weak",
            "Buyer"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }
}
