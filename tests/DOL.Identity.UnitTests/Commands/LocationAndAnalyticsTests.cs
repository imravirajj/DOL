using DOL.Identity.Application.DTOs;
using DOL.Identity.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace DOL.Identity.UnitTests.Commands;

public class LocationAndAnalyticsTests
{
    // ── Location Tests ──────────────────────────────────────────
    [Fact]
    public void Country_Creation_And_Update_Should_Work()
    {
        var companyId = Guid.NewGuid();
        var country = new Country(companyId, "India", "in");

        country.CompanyId.Should().Be(companyId);
        country.Name.Should().Be("India");
        country.IsoCode.Should().Be("IN");

        country.Update("Bharat", "in");
        country.Name.Should().Be("Bharat");
        country.IsoCode.Should().Be("IN");
    }

    [Fact]
    public void StateRegion_Creation_And_Update_Should_Work()
    {
        var companyId = Guid.NewGuid();
        var countryId = Guid.NewGuid();
        var state = new StateRegion(companyId, countryId, "Maharashtra", "mh");

        state.CompanyId.Should().Be(companyId);
        state.CountryId.Should().Be(countryId);
        state.Name.Should().Be("Maharashtra");
        state.StateCode.Should().Be("MH");

        state.Update("Maharashtra State", "mh");
        state.Name.Should().Be("Maharashtra State");
        state.StateCode.Should().Be("MH");
    }

    [Fact]
    public void City_Creation_And_Update_Should_Work()
    {
        var companyId = Guid.NewGuid();
        var stateId = Guid.NewGuid();
        var city = new City(companyId, stateId, "Mumbai");

        city.CompanyId.Should().Be(companyId);
        city.StateRegionId.Should().Be(stateId);
        city.Name.Should().Be("Mumbai");

        city.Update("Navi Mumbai");
        city.Name.Should().Be("Navi Mumbai");
    }

    // ── Analytics DTO Calculations ──────────────────────────────
    [Fact]
    public void SalesFunnelDto_Conversion_Percentages_Should_Be_Accurate()
    {
        int totalQuotations = 100;
        int totalOrders = 25;
        int completedDeliveries = 20;

        decimal leadToOrder = Math.Round(((decimal)totalOrders / totalQuotations) * 100m, 2);
        decimal orderToDelivery = Math.Round(((decimal)completedDeliveries / totalOrders) * 100m, 2);

        var funnel = new SalesFunnelDto(
            totalQuotations,
            totalOrders,
            PendingLoans: 5,
            ApprovedLoans: 18,
            completedDeliveries,
            leadToOrder,
            orderToDelivery);

        funnel.TotalQuotations.Should().Be(100);
        funnel.TotalOrders.Should().Be(25);
        funnel.LeadToOrderConversionPct.Should().Be(25.00m);
        funnel.OrderToDeliveryConversionPct.Should().Be(80.00m);
    }

    [Fact]
    public void StockAgingDto_Should_Track_Age_Buckets_Correctly()
    {
        var aging = new StockAgingDto(
            TotalVehiclesInStock: 45,
            Under30Days: 20,
            Between31And60Days: 15,
            Between61And90Days: 7,
            Over90Days: 3,
            TotalYardInventoryValue: 54000000m);

        aging.TotalVehiclesInStock.Should().Be(45);
        (aging.Under30Days + aging.Between31And60Days + aging.Between61And90Days + aging.Over90Days)
            .Should().Be(aging.TotalVehiclesInStock);
        aging.TotalYardInventoryValue.Should().Be(54000000m);
    }

    [Fact]
    public void RevenueAnalyticsDto_Should_Aggregate_Financial_Streams()
    {
        var revenue = new RevenueAnalyticsDto(
            TotalOrderValue: 12500000m,
            TotalBookingAmountCollected: 250000m,
            TotalDownPaymentCollected: 3000000m,
            TotalLoanDisbursed: 8500000m,
            TotalAccessoriesRevenue: 150000m,
            TotalServiceRevenue: 75000m);

        revenue.TotalOrderValue.Should().Be(12500000m);
        revenue.TotalBookingAmountCollected.Should().Be(250000m);
        revenue.TotalDownPaymentCollected.Should().Be(3000000m);
        revenue.TotalAccessoriesRevenue.Should().Be(150000m);
        revenue.TotalServiceRevenue.Should().Be(75000m);
    }
}
