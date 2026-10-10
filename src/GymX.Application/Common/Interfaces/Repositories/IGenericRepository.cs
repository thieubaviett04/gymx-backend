namespace GymX.Application.Common.Interfaces.Repositories;

/// <summary>
/// Bản hợp đồng "Mẹ" — định nghĩa các thao tác CRUD cơ bản dùng chung cho tất cả Entity.
/// Tầng Infrastructure sẽ implement interface này bằng EF Core.
/// </summary>
public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
}
