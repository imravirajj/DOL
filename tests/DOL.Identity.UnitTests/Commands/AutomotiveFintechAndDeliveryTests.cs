using DOL.Identity.Application.Commands.Delivery;
using DOL.Identity.Application.Commands.Loans;
using DOL.Identity.Application.Commands.TestDrives;
using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace DOL.Identity.UnitTests.Commands;

public class AutomotiveFintechAndDeliveryTests
{
    // ── EMI Calculator Tests ────────────────────────────────────
    [Fact]
    public async Task CalculateEmiQueryHandler_Should_Compute_Accurate_Emi()
    {
        var handler = new CalculateEmiQueryHandler();
        // Principal: 10 Lakhs, Rate: 9.0% per annum, Tenure: 60 months (5 years)
        var query = new CalculateEmiQuery(1000000m, 9.0m, 60);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        // Standard formula yields approx 20,758 per month
        result.Value.MonthlyEmi.Should().BeInRange(20750m, 20770m);
        result.Value.TotalAmountPayable.Should().BeGreaterThan(1000000m);
        result.Value.TotalInterestPayable.Should().Be(result.Value.TotalAmountPayable - 1000000m);
    }

    [Fact]
    public async Task CalculateEmiQueryHandler_Should_Fail_For_Zero_Or_Negative_Inputs()
    {
        var handler = new CalculateEmiQueryHandler();
        var query = new CalculateEmiQuery(0m, 9.0m, 60);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Invalid input values for EMI calculation.");
    }

    // ── Loan Application Tests ──────────────────────────────────
    [Fact]
    public void ApplyLoanCommandValidator_Should_Require_Valid_10_Char_Pan()
    {
        var validator = new ApplyLoanCommandValidator();
        var validCmd = new ApplyLoanCommand(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            800000m, 48, 75000m, "ABCDE1234F", "Salaried");

        var invalidPanCmd = new ApplyLoanCommand(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            800000m, 48, 75000m, "SHORTPAN", "Salaried");

        validator.Validate(validCmd).IsValid.Should().BeTrue();
        validator.Validate(invalidPanCmd).IsValid.Should().BeFalse();
    }

    [Fact]
    public void LoanApplication_Sanction_Should_Set_Approved_Details_And_Status()
    {
        var loan = new LoanApplication(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            1200000m, 60, 100000m, "ABCDE1234F", "Salaried");

        loan.Sanction("HDFC Bank", 1150000m, 8.75m, 23740m, "HDFC-AUTO-SANCTION-9988");

        loan.Status.Should().Be(LoanStatus.Sanctioned);
        loan.SelectedBankName.Should().Be("HDFC Bank");
        loan.ApprovedLoanAmount.Should().Be(1150000m);
        loan.ApprovedInterestRate.Should().Be(8.75m);
        loan.MonthlyEmi.Should().Be(23740m);
        loan.SanctionLetterNumber.Should().Be("HDFC-AUTO-SANCTION-9988");
    }

    // ── Vehicle Order & Delivery OTP Tests ──────────────────────
    [Fact]
    public void VehicleOrder_State_Machine_And_OTP_Verification_Should_Work()
    {
        var order = new VehicleOrder(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "ORD-2026-0001", 1850000m, 25000m, DeliveryType.ShowroomPickup);

        order.Status.Should().Be(OrderStatus.Booked);
        order.DeliveryOtp.Should().HaveLength(6);

        // Advance to VinAllocated -> PdiReady
        var stockId = Guid.NewGuid();
        order.AllocateStock(stockId);
        order.Status.Should().Be(OrderStatus.VinAllocated);
        order.AllocatedStockId.Should().Be(stockId);

        order.AdvanceStatus(OrderStatus.PdiReady);
        order.Status.Should().Be(OrderStatus.PdiReady);

        // Try wrong OTP
        bool wrongVerify = order.VerifyAndDeliver("000000");
        wrongVerify.Should().BeFalse();
        order.Status.Should().Be(OrderStatus.PdiReady);

        // Try correct OTP
        bool correctVerify = order.VerifyAndDeliver(order.DeliveryOtp);
        correctVerify.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Delivered);
        order.DeliveredAt.Should().NotBeNull();
    }

    // ── Test Drive Booking Tests ────────────────────────────────
    [Fact]
    public void TestDriveBooking_Should_Schedule_And_Complete_With_Rating()
    {
        var booking = new TestDriveBooking(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "Raviraj", "+919876543210", "ravi@test.com", "DL-MH-12345678",
            DateTime.UtcNow.AddDays(1), "11:00 AM - 12:00 PM");

        booking.Status.Should().Be(TestDriveStatus.Scheduled);

        // Reschedule
        var newDate = DateTime.UtcNow.AddDays(2);
        booking.Reschedule(newDate, "02:00 PM - 03:00 PM");
        booking.TimeSlot.Should().Be("02:00 PM - 03:00 PM");

        // Complete test drive
        booking.Complete(5, "Excellent acceleration and smooth suspension!");
        booking.Status.Should().Be(TestDriveStatus.Completed);
        booking.Rating.Should().Be(5);
        booking.FeedbackNotes.Should().Be("Excellent acceleration and smooth suspension!");
    }

    // ── Delivery Inspection (PDI) Tests ─────────────────────────
    [Fact]
    public void DeliveryInspection_Should_Capture_50_Point_Checklist_And_Signoff()
    {
        var pdi = new DeliveryInspection(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            12, 100, true, true, true, true, "All inspection points clear, no paint chips.");

        pdi.OdometerReadingKm.Should().Be(12);
        pdi.BatteryHealthPct.Should().Be(100);
        pdi.ExteriorConditionOk.Should().BeTrue();
        pdi.IsCustomerAccepted.Should().BeFalse();

        pdi.CustomerSignOff("https://cdn.dol.com/signatures/customer_sig_001.png");
        pdi.IsCustomerAccepted.Should().BeTrue();
        pdi.CustomerSignatureUrl.Should().Be("https://cdn.dol.com/signatures/customer_sig_001.png");
    }
}
