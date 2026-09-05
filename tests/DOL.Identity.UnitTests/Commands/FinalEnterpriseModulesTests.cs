using DOL.Identity.Application.Commands.Crm;
using DOL.Identity.Application.Commands.Documents;
using DOL.Identity.Application.Commands.Ev;
using DOL.Identity.Application.Commands.Payments;
using DOL.Identity.Application.Commands.Warranty;
using DOL.Identity.Domain.Entities;
using DOL.Identity.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace DOL.Identity.UnitTests.Commands;

public class FinalEnterpriseModulesTests
{
    // ── Payment Tests ───────────────────────────────────────────
    [Fact]
    public void PaymentTransaction_Success_And_Refund_Transitions()
    {
        var txn = new PaymentTransaction(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "TXN-2026-TEST-001", 25000m, PaymentPurpose.BookingToken,
            "Razorpay", "UPI");

        txn.Status.Should().Be(PaymentStatus.Initiated);
        txn.Amount.Should().Be(25000m);

        // Success
        txn.MarkSuccessful("pay_9988776655", "https://receipts.dol.com/pay_998877.pdf");
        txn.Status.Should().Be(PaymentStatus.Successful);
        txn.GatewayPaymentId.Should().Be("pay_9988776655");
        txn.PaidAt.Should().NotBeNull();

        // Refund
        txn.ProcessRefund("Customer requested variant change.");
        txn.Status.Should().Be(PaymentStatus.Refunded);
        txn.FailureReason.Should().Be("Customer requested variant change.");
    }

    [Fact]
    public void InitiatePaymentCommandValidator_Should_Validate_Required_Fields()
    {
        var validator = new InitiatePaymentCommandValidator();
        var valid = new InitiatePaymentCommand(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            11000m, PaymentPurpose.BookingToken);

        var invalidAmount = new InitiatePaymentCommand(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            0m, PaymentPurpose.BookingToken);

        validator.Validate(valid).IsValid.Should().BeTrue();
        validator.Validate(invalidAmount).IsValid.Should().BeFalse();
    }

    // ── Document KYC Tests ──────────────────────────────────────
    [Fact]
    public void CustomerDocument_Verification_And_Rejection_Transitions()
    {
        var staffId = Guid.NewGuid();
        var doc = new CustomerDocument(
            Guid.NewGuid(), Guid.NewGuid(), DocumentType.AadhaarCard,
            "XXXX-XXXX-1234", "https://storage.dol.com/kyc/aadhaar.pdf",
            "aadhaar.pdf", 512000);

        doc.VerificationStatus.Should().Be(DocumentVerificationStatus.Pending);

        // Verify
        doc.Verify(staffId);
        doc.VerificationStatus.Should().Be(DocumentVerificationStatus.Verified);
        doc.VerifiedByStaffId.Should().Be(staffId);
        doc.VerifiedAt.Should().NotBeNull();

        // Reject
        doc.Reject(staffId, "Name does not match booking quotation.");
        doc.VerificationStatus.Should().Be(DocumentVerificationStatus.Rejected);
        doc.RejectionReason.Should().Be("Name does not match booking quotation.");
    }

    // ── Warranty Tests ──────────────────────────────────────────
    [Fact]
    public void WarrantyPackage_And_Subscription_Dates_Calculation()
    {
        var companyId = Guid.NewGuid();
        var pkg = new WarrantyPackage(
            companyId, "Shield of Trust 5-Year", WarrantyPackageType.ExtendedWarranty,
            durationMonths: 60, kilometerLimit: 150000, price: 32000m,
            description: "Covers engine, gearbox, electricals.");

        pkg.DurationMonths.Should().Be(60);
        pkg.Price.Should().Be(32000m);

        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddMonths(pkg.DurationMonths);

        var sub = new VehicleWarrantySubscription(
            companyId, Guid.NewGuid(), Guid.NewGuid(), pkg.Id,
            "MALAA51BAM998877", "WRN-2026-001", startDate, endDate, pkg.Price);

        sub.Status.Should().Be(WarrantyStatus.Active);
        sub.EndDate.Should().BeAfter(sub.StartDate);

        // Cancel
        sub.Cancel();
        sub.Status.Should().Be(WarrantyStatus.Cancelled);
    }

    // ── Sales CRM Tests ─────────────────────────────────────────
    [Fact]
    public void SalesLead_Stage_Advancement_And_Staff_Assignment()
    {
        var lead = new SalesLead(
            Guid.NewGuid(), Guid.NewGuid(), "Rohan Verma", "+919876543210",
            "rohan@example.com", "WalkIn", LeadPriority.Hot);

        lead.Stage.Should().Be(LeadStage.New);
        lead.Priority.Should().Be(LeadPriority.Hot);

        // Assign
        var staffId = Guid.NewGuid();
        lead.AssignStaff(staffId);
        lead.AssignedStaffId.Should().Be(staffId);

        // Advance stage
        lead.AdvanceStage(LeadStage.TestDriveScheduled);
        lead.Stage.Should().Be(LeadStage.TestDriveScheduled);

        // Follow up
        var followUp = DateTime.UtcNow.AddDays(1);
        lead.ScheduleFollowUp(followUp, "Call after test drive at 4 PM.");
        lead.NextFollowUpDate.Should().Be(followUp);
        lead.Notes.Should().Be("Call after test drive at 4 PM.");
    }

    // ── EV Ecosystem Tests ──────────────────────────────────────
    [Fact]
    public void EvChargingStation_And_HomeCharger_Transitions()
    {
        var station = new EvChargingStation(
            Guid.NewGuid(), "Tata Power Fast Hub", "Andheri West Showroom",
            19.1136, 72.8697, "CCS2", 120, 21.0m);

        station.PowerKw.Should().Be(120);
        station.IsAvailable.Should().BeTrue();

        station.UpdateStatus(false, 22.5m);
        station.IsAvailable.Should().BeFalse();
        station.TariffPerKwh.Should().Be(22.5m);

        // Home Charger Survey
        var homeCharger = new HomeChargerInstallation(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "Flat 402, Sea Breeze, Bandra", DateTime.UtcNow.AddDays(2));

        homeCharger.SurveyStatus.Should().Be(HomeChargerSurveyStatus.Requested);

        homeCharger.UpdateSurvey(HomeChargerSurveyStatus.Installed, "Meter connected and test charged successfully.");
        homeCharger.SurveyStatus.Should().Be(HomeChargerSurveyStatus.Installed);
        homeCharger.InstalledAt.Should().NotBeNull();
        homeCharger.TechnicianNotes.Should().Contain("test charged successfully");
    }
}
