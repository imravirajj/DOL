using DOL.SharedKernel;

namespace DOL.Identity.Domain.Events;

public class QuotationGeneratedEvent : IDomainEvent
{
    public Guid QuotationId { get; }
    public string QuotationNumber { get; }
    public decimal TotalOnRoadPrice { get; }
    public string CustomerEmail { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public QuotationGeneratedEvent(Guid quotationId, string quotationNumber, decimal totalOnRoadPrice, string customerEmail)
    {
        QuotationId = quotationId;
        QuotationNumber = quotationNumber;
        TotalOnRoadPrice = totalOnRoadPrice;
        CustomerEmail = customerEmail;
    }
}
