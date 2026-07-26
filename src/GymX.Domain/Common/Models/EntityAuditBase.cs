
using GymX.Domain.Contracts;

namespace GymX.Domain.Common.Models;

public abstract class EntityAuditBase<TKey> : EntityBase<TKey>, IAuditable
{
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
}

public abstract class EntityAuditBase : EntityBase, IAuditable
{
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }

    protected EntityAuditBase() : base() { }
    protected EntityAuditBase(Guid id) : base(id) { }
}

