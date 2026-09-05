namespace DOL.Identity.Domain.Enums;

public enum WaitlistStatus
{
    Waiting = 1,              // In FIFO queue with token number
    Allocated = 2,            // Factory/cancelled vehicle assigned to this token
    CancelledAndRefunded = 3, // Customer opted out; 100% refund processed (terminal)
    Expired = 4               // Max waiting duration elapsed without factory supply (terminal)
}
