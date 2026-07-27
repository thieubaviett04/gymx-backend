namespace GymX.Domain.Contracts;

public interface IEntityBase<TKey>
{
    TKey Id { get; set; }
}

public interface IEntityBase : IEntityBase<Guid>
{
}
