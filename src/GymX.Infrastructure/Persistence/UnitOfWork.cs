using GymX.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;


namespace GymX.Infrastructure.Persistence
{
    public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
    {
        private IDbContextTransaction? currentTransaction;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (currentTransaction is not null) return;
            currentTransaction = await context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (currentTransaction is not null) await currentTransaction.CommitAsync(cancellationToken);
            }
            catch
            {
                if (currentTransaction is not null) await currentTransaction.RollbackAsync(cancellationToken);
            }
            finally
            {
                if (currentTransaction is not null) 
                await currentTransaction.DisposeAsync();
                currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (currentTransaction is not null) await currentTransaction.RollbackAsync(cancellationToken);
            }
            finally
            {
                if (currentTransaction is not null)
                await currentTransaction.DisposeAsync();
                currentTransaction = null;
            }

        }
    }
}
