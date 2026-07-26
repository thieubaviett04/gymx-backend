using GymX.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymX.Domain.Common.Models
{
    public abstract class EntityBase<TKey> : IEntityBase<TKey>
    {
        public TKey Id { get; set; } = default!;

        public override bool Equals(object? obj)
        {
            if (obj is not EntityBase<TKey> other)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (EqualityComparer<TKey>.Default.Equals(Id, default!) || EqualityComparer<TKey>.Default.Equals(other.Id, default!))
            {
                return false;
            }

            return EqualityComparer<TKey>.Default.Equals(Id, other.Id);
        }

        public override int GetHashCode() => Id?.GetHashCode() ?? 0;

    }

    public abstract class EntityBase : EntityBase<Guid>, IEntityBase
    {
        protected EntityBase()
        {
            Id = Guid.NewGuid();
        }

        protected EntityBase(Guid id)
        {
            Id = id;
        }
    }

}
