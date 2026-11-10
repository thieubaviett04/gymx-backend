using GymX.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;

namespace GymX.Infrastructure.Persistence.Interceptors
{
    public class UniqueConstraintInterceptor : SaveChangesInterceptor
    {
        public override void SaveChangesFailed(DbContextErrorEventData eventData)
        {
            HandleException(eventData.Exception);
            base.SaveChangesFailed(eventData);
        }

        public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
        {
            HandleException(eventData.Exception);
            return base.SaveChangesFailedAsync(eventData, cancellationToken);
        }

        private static void HandleException(Exception? exception)
        {
            if (exception is DbUpdateException dbUpdateException &&
                dbUpdateException.InnerException is PostgresException postgresException &&
                postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                throw new UniqueConstraintException("Đã xảy ra lỗi vi phạm ràng buộc dữ liệu duy nhất khi lưu thay đổi.", dbUpdateException);
            }
        }
    }
}

