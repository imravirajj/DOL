namespace DOL.Identity.Domain.Enums;

public enum QuotationStatus
{
    Active = 1,                 // Valid and price locked
    ConvertedToBooking = 2,     // Customer clicked proceed & reserved car
    Expired = 3,                // Passed validity date (e.g. 7 days)
    Cancelled = 4               // Revoked or superseded by a new quotation
}
