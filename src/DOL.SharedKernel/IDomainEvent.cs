namespace DOL.SharedKernel;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
