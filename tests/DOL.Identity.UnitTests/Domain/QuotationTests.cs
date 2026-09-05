using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace DOL.Identity.UnitTests.Domain;

public class QuotationTests
{
    [Fact]
    public void Quotation_PriceCalculation_ShouldSumAllComponentsAndSubtractDiscount()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

        var exShowroom = 1200000m;
        var rto = 120000m;
        var baseInsurance = 42000m;
        var addonsInsurance = 9600m;
        var fastag = 500m;
        var tcs = 12000m;
        var accessories = 15000m;
        var warranty = 14400m;
        var discount = 25000m;

        var expectedOnRoad = (exShowroom + rto + baseInsurance + addonsInsurance + fastag + tcs + accessories + warranty) - discount;

        // Act
        var quotation = new Quotation(
            companyId,
            branchId,
            variantId,
            null,
            "QTN-2026-MUM-0001",
            "Amit Sharma",
            "amit@example.com",
            "+919876543210",
            "Daytona Grey",
            exShowroom,
            rto,
            baseInsurance,
            addonsInsurance,
            fastag,
            tcs,
            accessories,
            warranty,
            discount,
            includeZeroDep: true,
            includeEngineProtect: true,
            includeReturnToInvoice: false,
            includeExtendedWarranty: true,
            null,
            TimeSpan.FromDays(7)
        );

        // Assert
        quotation.TotalOnRoadPrice.Should().Be(expectedOnRoad);
        quotation.Status.Should().Be(QuotationStatus.Active);
        quotation.ValidUntil.Should().BeAfter(DateTime.UtcNow.AddDays(6));
    }

    [Fact]
    public void Quotation_MarkConvertedToBooking_ShouldUpdateStatus()
    {
        // Arrange
        var quotation = new Quotation(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null,
            "QTN-2026-MUM-0002", "Priya Patel", "priya@example.com", "+919876543211", "White",
            800000m, 80000m, 28000m, 6400m, 500m, 0m, 5000m, 0m, 10000m,
            true, false, false, false, null, TimeSpan.FromDays(7)
        );

        // Act
        quotation.MarkConvertedToBooking();

        // Assert
        quotation.Status.Should().Be(QuotationStatus.ConvertedToBooking);
    }

    [Fact]
    public void Quotation_Expire_WhenPastValidity_ShouldTransitionToExpired()
    {
        // Arrange
        var quotation = new Quotation(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null,
            "QTN-2026-MUM-0003", "Rahul Roy", "rahul@example.com", "+919876543212", "Black",
            1000000m, 100000m, 35000m, 0m, 500m, 10000m, 0m, 0m, 0m,
            false, false, false, false, null, TimeSpan.FromSeconds(-1) // expired immediately
        );

        // Act
        quotation.Expire();

        // Assert
        quotation.Status.Should().Be(QuotationStatus.Expired);
    }
}
