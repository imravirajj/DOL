using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Events;
using FluentAssertions;
using Xunit;

namespace DOL.Identity.UnitTests.Domain;

public class ApplicationUserTests
{
    [Fact]
    public void ApplicationUser_Creation_ShouldSetPropertiesAndRaiseUserRegisteredEvent()
    {
        // Arrange & Act
        var user = new ApplicationUser(
            "John",
            "Doe",
            "john.doe@example.com",
            "+1234567890",
            "hashed_password_123"
        );

        // Assert
        user.FirstName.Should().Be("John");
        user.LastName.Should().Be("Doe");
        user.Email.Should().Be("john.doe@example.com");
        user.PhoneNumber.Should().Be("+1234567890");
        user.PasswordHash.Should().Be("hashed_password_123");
        user.DomainEvents.Should().ContainSingle(e => e is UserRegisteredEvent);
    }

    [Fact]
    public void RecordFailedLogin_5Times_ShouldLockoutUser()
    {
        // Arrange
        var user = new ApplicationUser("John", "Doe", "john@example.com", "+1234567890", "hash");

        // Act
        for (int i = 0; i < 5; i++)
        {
            user.RecordFailedLogin();
        }

        // Assert
        user.IsLockedOut.Should().BeTrue();
        user.LockoutEnd.Should().NotBeNull();
    }
}
