using DOL.SharedKernel;

namespace DOL.Identity.Domain.Entities;

public class DealershipReview : AuditableEntity, IBranchScoped
{
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid BuyerId { get; private set; }
    public Guid? OrderId { get; private set; }

    public int Rating { get; private set; } // 1 to 5
    public string Title { get; private set; } = string.Empty;
    public string ReviewText { get; private set; } = string.Empty;
    public bool IsVerifiedBuyer { get; private set; } = true;
    public string? DealerResponse { get; private set; }
    public DateTime? RespondedAt { get; private set; }

    public ApplicationUser? Buyer { get; private set; }
    public VehicleOrder? Order { get; private set; }

    private DealershipReview() { } // EF Core

    public DealershipReview(
        Guid companyId,
        Guid branchId,
        Guid buyerId,
        int rating,
        string title,
        string reviewText,
        Guid? orderId = null,
        bool isVerifiedBuyer = true)
    {
        CompanyId = companyId;
        BranchId = branchId;
        BuyerId = buyerId;
        Rating = Math.Clamp(rating, 1, 5);
        Title = title.Trim();
        ReviewText = reviewText.Trim();
        OrderId = orderId;
        IsVerifiedBuyer = isVerifiedBuyer;
    }

    public void Respond(string response)
    {
        DealerResponse = response.Trim();
        RespondedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
