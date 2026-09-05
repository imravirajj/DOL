using DOL.Identity.Application.Commands.Accessories;
using DOL.Identity.Application.Commands.Analytics;
using DOL.Identity.Application.Commands.Exchange;
using DOL.Identity.Application.Commands.Insurance;
using DOL.Identity.Application.Commands.Notifications;
using DOL.Identity.Application.Commands.Reviews;
using DOL.Identity.Application.Commands.Service;
using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace DOL.Identity.UnitTests.Commands;

public class EnterpriseDealershipModulesTests
{
    // ── Trade-In / Exchange Tests ───────────────────────────────
    [Fact]
    public void VehicleTradeIn_State_Transitions_Should_Work_Correctly()
    {
        var tradeIn = new VehicleTradeIn(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "Hyundai", "Creta", 2021, 45000, "Petrol", "Good", false,
            estimatedValue: 650000m, "MH02AB1234");

        tradeIn.Status.Should().Be(TradeInStatus.Valuated);
        tradeIn.EstimatedValue.Should().Be(650000m);

        // Schedule inspection
        var inspectionDate = DateTime.UtcNow.AddDays(2);
        tradeIn.ScheduleInspection(inspectionDate);
        tradeIn.Status.Should().Be(TradeInStatus.InspectionScheduled);
        tradeIn.InspectionDate.Should().Be(inspectionDate);

        // Offer
        tradeIn.ProvideOffer(640000m, "Minor scratches on rear bumper.");
        tradeIn.OfferedValue.Should().Be(640000m);
        tradeIn.InspectorNotes.Should().Be("Minor scratches on rear bumper.");

        // Accept
        tradeIn.AcceptOffer();
        tradeIn.Status.Should().Be(TradeInStatus.OfferAccepted);

        // Complete
        tradeIn.CompleteTradeIn();
        tradeIn.Status.Should().Be(TradeInStatus.Completed);
    }

    [Fact]
    public void ValuateTradeInCommandValidator_Should_Validate_Required_Fields()
    {
        var validator = new ValuateTradeInCommandValidator();
        var valid = new ValuateTradeInCommand(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "Tata", "Harrier", 2022, 28000, "Diesel", "Excellent", false);

        var invalidYear = new ValuateTradeInCommand(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "Tata", "Harrier", 1995, 28000, "Diesel", "Excellent", false);

        validator.Validate(valid).IsValid.Should().BeTrue();
        validator.Validate(invalidYear).IsValid.Should().BeFalse();
    }

    // ── Accessories Tests ───────────────────────────────────────
    [Fact]
    public void VehicleAccessory_Should_Initialize_And_Update_Correctly()
    {
        var companyId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var accessory = new VehicleAccessory(
            companyId, "All-Weather 7D Floor Mats", "ACC-MAT-7D-01",
            AccessoryCategory.Interior, price: 4999m, installationCost: 500m,
            warrantyMonths: 24, compatibleVariantId: variantId);

        accessory.Name.Should().Be("All-Weather 7D Floor Mats");
        accessory.PartNumber.Should().Be("ACC-MAT-7D-01");
        accessory.Category.Should().Be(AccessoryCategory.Interior);
        accessory.Price.Should().Be(4999m);
        accessory.InstallationCost.Should().Be(500m);
        accessory.IsActive.Should().BeTrue();

        // Update
        accessory.Update("All-Weather 7D Floor Mats - Premium", AccessoryCategory.Interior, 5499m, 400m, 36, variantId, true);
        accessory.Name.Should().Be("All-Weather 7D Floor Mats - Premium");
        accessory.Price.Should().Be(5499m);
        accessory.WarrantyMonths.Should().Be(36);
    }

    // ── Insurance Tests ─────────────────────────────────────────
    [Fact]
    public async Task GetInsurancePlansQueryHandler_Should_Return_Top_Insurer_Quotes()
    {
        var handler = new GetInsurancePlansQueryHandler();
        var query = new GetInsurancePlansQuery(1200000m); // 12 Lakhs car

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        result.Value.Count.Should().Be(4);

        var hdfc = result.Value.First(p => p.InsurerName.Contains("HDFC"));
        hdfc.AnnualPremium.Should().BeGreaterThan(30000m);
        hdfc.ZeroDepIncluded.Should().BeTrue();
    }

    [Fact]
    public void InsurancePolicy_Issue_And_Cancel_Transitions()
    {
        var policy = new InsurancePolicy(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "ICICI Lombard", "Comprehensive Zero-Dep", 34500m, 1150000m,
            DateTime.UtcNow, DateTime.UtcNow.AddYears(1));

        policy.Status.Should().Be(InsurancePolicyStatus.Draft);

        // Issue
        policy.IssuePolicy("POL-2026-ICICI-998877", "https://storage.dol.com/policies/998877.pdf");
        policy.Status.Should().Be(InsurancePolicyStatus.Active);
        policy.PolicyNumber.Should().Be("POL-2026-ICICI-998877");
        policy.PolicyDocumentUrl.Should().Be("https://storage.dol.com/policies/998877.pdf");

        // Cancel
        policy.CancelPolicy();
        policy.Status.Should().Be(InsurancePolicyStatus.Cancelled);
    }

    // ── Service Appointment Tests ───────────────────────────────
    [Fact]
    public void ServiceAppointment_Lifecycle_Transitions()
    {
        var appt = new ServiceAppointment(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "MALAA51BAM1234567", "MH01DE4321", ServiceType.PeriodicMaintenance,
            DateTime.UtcNow.AddDays(3), "11:00 AM - 01:00 PM",
            estimatedCost: 3500m, "Oil filter change and brake inspection.");

        appt.Status.Should().Be(ServiceAppointmentStatus.Scheduled);
        appt.VinNumber.Should().Be("MALAA51BAM1234567");

        // Start service
        appt.StartService();
        appt.Status.Should().Be(ServiceAppointmentStatus.InProgress);

        // Complete service
        appt.CompleteService(3850m, "Replaced synthetic engine oil and oil filter. Brake pads good.");
        appt.Status.Should().Be(ServiceAppointmentStatus.Completed);
        appt.ActualCost.Should().Be(3850m);
        appt.CompletedAt.Should().NotBeNull();
    }

    // ── Notification Tests ──────────────────────────────────────
    [Fact]
    public void CustomerNotification_MarkAsRead_Should_Set_ReadAt()
    {
        var notification = new CustomerNotification(
            Guid.NewGuid(), Guid.NewGuid(),
            "Vehicle Dispatched to Showroom",
            "Your Hyundai Creta Dark Edition is loaded onto the transporter carrier.",
            NotificationChannel.InApp,
            "/orders/track");

        notification.IsRead.Should().BeFalse();
        notification.ReadAt.Should().BeNull();

        notification.MarkAsRead();
        notification.IsRead.Should().BeTrue();
        notification.ReadAt.Should().NotBeNull();
    }

    // ── Review & NPS Tests ──────────────────────────────────────
    [Fact]
    public void DealershipReview_Rating_Should_Be_Clamped_Between_1_And_5()
    {
        var reviewHigh = new DealershipReview(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            rating: 10, title: "Amazing Service", reviewText: "Delivered right on time!");

        var reviewLow = new DealershipReview(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            rating: -2, title: "Poor Delivery", reviewText: "Staff was unhelpful.");

        reviewHigh.Rating.Should().Be(5);
        reviewLow.Rating.Should().Be(1);

        // Dealer Response
        reviewHigh.Respond("Thank you for choosing DOL dealership! Enjoy your new ride.");
        reviewHigh.DealerResponse.Should().Be("Thank you for choosing DOL dealership! Enjoy your new ride.");
        reviewHigh.RespondedAt.Should().NotBeNull();
    }
}
