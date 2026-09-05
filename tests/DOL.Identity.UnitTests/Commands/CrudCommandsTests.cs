using DOL.Identity.Application.Commands.Branches;
using DOL.Identity.Application.Commands.Companies;
using DOL.Identity.Application.Commands.Inventory;
using DOL.Identity.Application.Commands.Rto;
using DOL.Identity.Application.Commands.Users;
using DOL.Identity.Application.Commands.Vehicles;
using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace DOL.Identity.UnitTests.Commands;

public class CrudCommandsTests
{
    // ── Company CRUD Tests ──────────────────────────────────────
    [Fact]
    public void UpdateCompanyCommandValidator_Should_Pass_For_Valid_Data()
    {
        var validator = new UpdateCompanyCommandValidator();
        var cmd = new UpdateCompanyCommand(Guid.NewGuid(), "Tata Motors Ltd", "+919876543210", "Mumbai HQ", "INR", "Asia/Kolkata");

        var result = validator.Validate(cmd);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void UpdateCompanyCommandValidator_Should_Fail_When_Name_Or_Phone_Empty()
    {
        var validator = new UpdateCompanyCommandValidator();
        var cmd = new UpdateCompanyCommand(Guid.Empty, "", "", null, "", "");

        var result = validator.Validate(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Company_UpdateDetails_And_SetStatus_Should_Modify_Entity()
    {
        var company = new Company("Old Motors", "OLD-01", "info@old.com", "+911111111111");
        company.UpdateDetails("New Motors", "+919999999999", "New Address", "INR", "Asia/Kolkata");
        company.SetStatus(CompanyStatus.Suspended);

        company.Name.Should().Be("New Motors");
        company.PhoneNumber.Should().Be("+919999999999");
        company.Address.Should().Be("New Address");
        company.Status.Should().Be(CompanyStatus.Suspended);
    }

    // ── Branch CRUD Tests ───────────────────────────────────────
    [Fact]
    public void UpdateBranchCommandValidator_Should_Pass_For_Valid_Data()
    {
        var validator = new UpdateBranchCommandValidator();
        var cmd = new UpdateBranchCommand(Guid.NewGuid(), "Andheri West Flagship", "Link Road, Mumbai", "+919876500000", "andheri@tatamotors.com");

        var result = validator.Validate(cmd);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Branch_UpdateDetails_And_SetActiveStatus_Should_Modify_Entity()
    {
        var branch = new Branch(Guid.NewGuid(), Guid.NewGuid(), "Old Branch", "BR-01", "Old Address");
        branch.UpdateDetails("Updated Branch", "New Street", "+912233445566", "branch@motors.com");
        branch.SetActiveStatus(false);

        branch.Name.Should().Be("Updated Branch");
        branch.Address.Should().Be("New Street");
        branch.ContactPhone.Should().Be("+912233445566");
        branch.IsActive.Should().BeFalse();
    }

    // ── Vehicle Catalog CRUD Tests ──────────────────────────────
    [Fact]
    public void CreateVehicleModelCommandValidator_Should_Pass_For_Valid_Model()
    {
        var validator = new CreateVehicleModelCommandValidator();
        var cmd = new CreateVehicleModelCommand(Guid.NewGuid(), "Tata", "Harrier EV", 2026, "EV");

        var result = validator.Validate(cmd);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateVehicleVariantCommandValidator_Should_Validate_Price_GreaterThan_Zero()
    {
        var validator = new CreateVehicleVariantCommandValidator();
        var validCmd = new CreateVehicleVariantCommand(Guid.NewGuid(), Guid.NewGuid(), "XZA+ Dark", "Diesel", "Automatic", 2650000m, "Black,Grey");
        var invalidCmd = new CreateVehicleVariantCommand(Guid.NewGuid(), Guid.NewGuid(), "XZA+ Dark", "Diesel", "Automatic", 0m, "Black,Grey");

        validator.Validate(validCmd).IsValid.Should().BeTrue();
        validator.Validate(invalidCmd).IsValid.Should().BeFalse();
    }

    [Fact]
    public void VehicleModel_And_Variant_UpdateDetails_Should_Modify_Entities()
    {
        var model = new VehicleModel(Guid.NewGuid(), "Tata", "Nexon", 2025, "SUV");
        model.UpdateDetails("Tata", "Nexon Facelift", 2026, "Compact SUV");
        model.SetActiveStatus(true);

        model.Model.Should().Be("Nexon Facelift");
        model.Year.Should().Be(2026);
        model.Category.Should().Be("Compact SUV");

        var variant = new VehicleVariant(Guid.NewGuid(), model.Id, "Creative", "Petrol", "Manual", 1100000m, "White,Blue");
        variant.UpdateDetails("Creative Plus AT", "Petrol", "Automatic", 1250000m, "White,Blue,Red");

        variant.VariantName.Should().Be("Creative Plus AT");
        variant.Transmission.Should().Be("Automatic");
        variant.ExShowroomPrice.Should().Be(1250000m);
    }

    // ── Inventory Stock CRUD Tests ──────────────────────────────
    [Fact]
    public void AddVehicleStockCommandValidator_Should_Require_17_Char_VIN()
    {
        var validator = new AddVehicleStockCommandValidator();
        var validCmd = new AddVehicleStockCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "MAT12345678901234", "ENG998877", "Daytona Grey");
        var shortVinCmd = new AddVehicleStockCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "SHORTVIN", "ENG998877", "Daytona Grey");

        validator.Validate(validCmd).IsValid.Should().BeTrue();
        validator.Validate(shortVinCmd).IsValid.Should().BeFalse();
    }

    [Fact]
    public void VehicleStock_UpdateDetails_And_SetStatus_Should_Work()
    {
        var stock = new VehicleStock(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "VIN12345678901234", "ENG111", "White");
        stock.UpdateDetails("Metallic Black", "ENG111-MOD");
        stock.SetStatus(VehicleStockStatus.InTransit);

        stock.Color.Should().Be("Metallic Black");
        stock.EngineNumber.Should().Be("ENG111-MOD");
        stock.Status.Should().Be(VehicleStockStatus.InTransit);
    }

    // ── User Management CRUD Tests ──────────────────────────────
    [Fact]
    public void UpdateUserCommandValidator_Should_Require_Name_And_Phone()
    {
        var validator = new UpdateUserCommandValidator();
        var valid = new UpdateUserCommand(Guid.NewGuid(), "Ravi", "Raj", "+919876543210");
        var invalid = new UpdateUserCommand(Guid.Empty, "", "", "");

        validator.Validate(valid).IsValid.Should().BeTrue();
        validator.Validate(invalid).IsValid.Should().BeFalse();
    }

    [Fact]
    public void ApplicationUser_UpdateProfile_And_SetStatus_Should_Modify_User()
    {
        var user = new ApplicationUser("Ravi", "Old", "ravi@test.com", "+911111111111", "hash123");
        user.UpdateProfile("Raviraj", "Kumar", "+919876543210");
        user.SetStatus(UserStatus.Suspended);

        user.FirstName.Should().Be("Raviraj");
        user.LastName.Should().Be("Kumar");
        user.PhoneNumber.Should().Be("+919876543210");
        user.Status.Should().Be(UserStatus.Suspended);
    }

    // ── RTO Tax Slab CRUD Tests ─────────────────────────────────
    [Fact]
    public void CreateRtoTaxSlabCommandValidator_Should_Pass_For_Valid_Data()
    {
        var validator = new CreateRtoTaxSlabCommandValidator();
        var valid = new CreateRtoTaxSlabCommand(Guid.NewGuid(), "Maharashtra", "Petrol", 11.0m, 1.0m);
        var invalid = new CreateRtoTaxSlabCommand(Guid.Empty, "", "", -5m, -1m);

        validator.Validate(valid).IsValid.Should().BeTrue();
        validator.Validate(invalid).IsValid.Should().BeFalse();
    }

    [Fact]
    public void RtoTaxSlab_Update_Should_Modify_Percentages()
    {
        var slab = new RtoTaxSlab(Guid.NewGuid(), "Delhi", "EV", 0m, 0m);
        slab.Update(2.5m, 0.5m);

        slab.TaxPercentage.Should().Be(2.5m);
        slab.CessPercentage.Should().Be(0.5m);
    }
}
