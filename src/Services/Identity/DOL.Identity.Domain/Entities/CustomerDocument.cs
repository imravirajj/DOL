using DOL.Identity.Domain.Enums;
using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class CustomerDocument : AuditableEntity, ITenantScoped
{
    public Guid CompanyId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid? OrderId { get; private set; }

    public DocumentType DocumentType { get; private set; } = DocumentType.AadhaarCard;
    public string DocumentNumber { get; private set; } = string.Empty;
    public string FileUrl { get; private set; } = string.Empty;
    public string FileName { get; private set; } = string.Empty;
    public long FileSizeBytes { get; private set; }

    public DocumentVerificationStatus VerificationStatus { get; private set; } = DocumentVerificationStatus.Pending;
    public Guid? VerifiedByStaffId { get; private set; }
    public DateTime? VerifiedAt { get; private set; }
    public string? RejectionReason { get; private set; }

    public ApplicationUser? User { get; private set; }
    public VehicleOrder? Order { get; private set; }

    private CustomerDocument() { } // EF Core

    public CustomerDocument(
        Guid companyId,
        Guid userId,
        DocumentType documentType,
        string documentNumber,
        string fileUrl,
        string fileName,
        long fileSizeBytes,
        Guid? orderId = null)
    {
        CompanyId = companyId;
        UserId = userId;
        DocumentType = documentType;
        DocumentNumber = documentNumber.Trim().ToUpperInvariant();
        FileUrl = fileUrl.Trim();
        FileName = fileName.Trim();
        FileSizeBytes = fileSizeBytes;
        OrderId = orderId;
        VerificationStatus = DocumentVerificationStatus.Pending;
    }

    public void Verify(Guid staffId)
    {
        VerificationStatus = DocumentVerificationStatus.Verified;
        VerifiedByStaffId = staffId;
        VerifiedAt = DateTime.UtcNow;
        RejectionReason = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(Guid staffId, string reason)
    {
        VerificationStatus = DocumentVerificationStatus.Rejected;
        VerifiedByStaffId = staffId;
        VerifiedAt = DateTime.UtcNow;
        RejectionReason = reason.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}
