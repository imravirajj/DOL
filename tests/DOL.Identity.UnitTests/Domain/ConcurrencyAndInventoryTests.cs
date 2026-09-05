using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace DOL.Identity.UnitTests.Domain;

public class ConcurrencyAndInventoryTests
{
    [Fact]
    public void TryReserve_WhenCarIsAvailable_ShouldLockFor15Minutes()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var buyerId = Guid.NewGuid();

        var stock = new VehicleStock(companyId, branchId, variantId, "VIN1234567890ABCDE", "ENG998877", "Carbon Black");

        // Act
        var reserved = stock.TryReserve(buyerId, TimeSpan.FromMinutes(15));

        // Assert
        reserved.Should().BeTrue();
        stock.Status.Should().Be(VehicleStockStatus.Reserved);
        stock.ReservedByBuyerId.Should().Be(buyerId);
        stock.ReservationExpiresAt.Should().BeAfter(DateTime.UtcNow.AddMinutes(14));
    }

    [Fact]
    public void TryReserve_WhenTwoBuyersCompete_SecondBuyerShouldBeLockedOut()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var buyer1 = Guid.NewGuid();
        var buyer2 = Guid.NewGuid();

        var stock = new VehicleStock(companyId, branchId, variantId, "VIN1234567890ABCDE", "ENG998877", "Carbon Black");

        // Act: Buyer 1 locks the car first
        var buyer1Result = stock.TryReserve(buyer1, TimeSpan.FromMinutes(15));

        // Act: Buyer 2 tries to reserve the exact same car
        var buyer2Result = stock.TryReserve(buyer2, TimeSpan.FromMinutes(15));

        // Assert
        buyer1Result.Should().BeTrue();
        buyer2Result.Should().BeFalse(); // Buyer 2 is locked out!
        stock.ReservedByBuyerId.Should().Be(buyer1); // Still held exclusively by Buyer 1
    }

    [Fact]
    public void TryReserve_When15MinuteHoldExpires_SecondBuyerCanClaimIt()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var buyer1 = Guid.NewGuid();
        var buyer2 = Guid.NewGuid();

        var stock = new VehicleStock(companyId, branchId, variantId, "VIN1234567890ABCDE", "ENG998877", "Carbon Black");

        // Buyer 1 reserved it, but hold was 0 seconds (expired immediately)
        stock.TryReserve(buyer1, TimeSpan.FromSeconds(-1));

        // Act: Buyer 2 tries to reserve
        var buyer2Result = stock.TryReserve(buyer2, TimeSpan.FromMinutes(15));

        // Assert: Buyer 2 succeeds because Buyer 1's hold expired!
        buyer2Result.Should().BeTrue();
        stock.ReservedByBuyerId.Should().Be(buyer2);
    }

    [Fact]
    public void ConfirmBooking_WithinHoldWindow_ShouldPermanentlyLockVin()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var buyer = Guid.NewGuid();
        var orderId = Guid.NewGuid();

        var stock = new VehicleStock(companyId, branchId, variantId, "VIN1234567890ABCDE", "ENG998877", "Carbon Black");
        stock.TryReserve(buyer, TimeSpan.FromMinutes(15));

        // Act
        var confirmed = stock.ConfirmBooking(buyer, orderId);

        // Assert
        confirmed.Should().BeTrue();
        stock.Status.Should().Be(VehicleStockStatus.Booked);
        stock.ConfirmedOrderId.Should().Be(orderId);
        stock.ReservationExpiresAt.Should().BeNull(); // Permanent booking, no longer timer-based
    }

    [Fact]
    public void WaitlistEntry_QueueAllocationAnd1ClickRefund_ShouldWorkCleanly()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var buyerId = Guid.NewGuid();

        var waitlist = new WaitlistEntry(companyId, branchId, variantId, buyerId, 1, 11000m, "IDEM-KEY-001");

        // Assert Initial State
        waitlist.QueuePosition.Should().Be(1);
        waitlist.Status.Should().Be(WaitlistStatus.Waiting);

        // Act 1: 1-Click refund
        waitlist.CancelAndRefund();

        // Assert: Terminal state
        waitlist.Status.Should().Be(WaitlistStatus.CancelledAndRefunded);
    }
}
