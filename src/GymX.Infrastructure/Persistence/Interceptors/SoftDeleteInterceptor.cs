using GymX.Application.Common.Interfaces;
using GymX.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;


namespace GymX.Infrastructure.Persistence.Interceptors
{
    public class SoftDeleteInterceptor(IDateTimeProvider dateTimeProvider) : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            ApplySoftDelete(eventData.Context);
            return base.SavingChanges(eventData, result);
        }
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            ApplySoftDelete(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
        private void ApplySoftDelete(DbContext? context)
        {
            if (context == null) return;
            foreach (var entry in context.ChangeTracker.Entries<ISoftDelete>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedDate = dateTimeProvider.UtcNow;
                }
            }
        }
    }
}
