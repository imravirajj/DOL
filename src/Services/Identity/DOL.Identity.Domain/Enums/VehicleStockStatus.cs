namespace DOL.Identity.Domain.Enums;

public enum VehicleStockStatus
{
    Available = 1,
    Reserved = 2,    // 15-min temporary lock while buyer completes payment
    Booked = 3,      // Payment successful, permanently allocated to order
    InTransit = 4,   // Moving from factory or inter-branch transfer
    Delivered = 5    // Handed over to buyer (terminal state)
}
