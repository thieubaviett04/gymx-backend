using GymX.Application.Common.Interfaces;
using GymX.Infrastructure.Options;
using GymX.Infrastructure.Persistence;
using GymX.Infrastructure.Persistence.Interceptors;
using GymX.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GymX.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.SectionName));
            services.Configure<MailOptions>(configuration.GetSection(MailOptions.SectionName));
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<AuditableEntityInterceptor>();
            services.AddScoped<SoftDeleteInterceptor>();
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
                options.UseNpgsql(connectionString, npgsql =>
                {
                    npgsql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    npgsql.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                });
                var auditInterceptor = sp.GetRequiredService<AuditableEntityInterceptor>();
                var softDeleteInterceptor = sp.GetRequiredService<SoftDeleteInterceptor>();
                options.AddInterceptors(auditInterceptor, softDeleteInterceptor);
            });
            services.AddScoped<IApplicationDbContext>(sp =>
                sp.GetRequiredService<ApplicationDbContext>());
            return services;
        }
    }
}
