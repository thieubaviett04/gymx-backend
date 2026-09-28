using GymX.Application.Common.Interfaces;
using GymX.Infrastructure.Options;
using GymX.Infrastructure.Persistence;
using GymX.Infrastructure.Persistence.Interceptors;
using GymX.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GymX.Application.Common.Interfaces.Authencation;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Infrastructure.Repositories;
using GymX.Infrastructure.Services.Authentication;
using Microsoft.Extensions.Options;
using PayOS;

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
                
            });
            services.AddScoped<IApplicationDbContext>(sp =>
                sp.GetRequiredService<ApplicationDbContext>());

            services.AddScoped<IUnitOfWork>(sp =>
                sp.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IGoogleAuthService, GoogleAuthService>();    
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IEmailService, EmailService>();

            // Face++ Face Recognition
            services.Configure<FacePlusPlusOptions>(configuration.GetSection(FacePlusPlusOptions.SectionName));
            services.AddHttpClient<IFaceRecognitionService, FaceRecognitionService>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            // PayOS Payment Gateway
            services.Configure<GymX.Infrastructure.Options.PayOSOptions>(configuration.GetSection(GymX.Infrastructure.Options.PayOSOptions.SectionName));
            services.AddSingleton<PayOSClient>(sp =>
            {
                var opts = sp.GetRequiredService<IOptions<GymX.Infrastructure.Options.PayOSOptions>>().Value;
                return new PayOSClient(opts.ClientId, opts.ApiKey, opts.ChecksumKey);
            });
            services.AddScoped<IPaymentService, PaymentService>();

            return services;
        }
    }
}
