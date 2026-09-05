using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace DOL.Identity.UnitTests.Domain;

public class CompanyAndBranchTests
{
    [Fact]
    public void Company_Creation_ShouldSetProperties()
    {
        var company = new Company(
            "Acme Motors",
            "ACME",
            "info@acmemotors.com",
            "+1234567890",
            "100 Main St",
            "Enterprise",
            "USD",
            "UTC"
        );

        company.Name.Should().Be("Acme Motors");
        company.Code.Should().Be("ACME");
        company.Email.Should().Be("info@acmemotors.com");
        company.Status.Should().Be(CompanyStatus.Active);
    }

    [Fact]
    public void Branch_Creation_ShouldImplementBranchScoped()
    {
        var companyId = Guid.NewGuid();
        var cityId = Guid.NewGuid();

        var branch = new Branch(
            companyId,
            cityId,
            "Mumbai Central Branch",
            "MUM-01",
            "Downtown Mumbai",
            "+919876543210",
            "mumbai@acmemotors.com",
            isMainBranch: true
        );

        branch.CompanyId.Should().Be(companyId);
        branch.BranchId.Should().Be(branch.Id);
        branch.CityId.Should().Be(cityId);
        branch.BranchCode.Should().Be("MUM-01");
        branch.IsMainBranch.Should().BeTrue();
        branch.IsActive.Should().BeTrue();
    }

    [Fact]
    public void User_AssignedToBranch_ShouldHaveCorrectBranchScope()
    {
        var companyId = Guid.NewGuid();
        var branchId = Guid.NewGuid();

        var user = new ApplicationUser(
            "Branch",
            "Manager",
            "manager@acmemotors.com",
            "+1234567890",
            "hashed_password",
            companyId,
            AccessScope.BranchLevel,
            branchId,
            branchId
        );

        user.CompanyId.Should().Be(companyId);
        user.BranchId.Should().Be(branchId);
        user.Scope.Should().Be(AccessScope.BranchLevel);
        user.ScopeEntityId.Should().Be(branchId);
    }
}
