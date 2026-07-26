namespace GymX.Domain.Contracts;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
