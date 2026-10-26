using System.Text;
using GymX.Api.Common.Middleware;
using GymX.Api.Hubs;
using GymX.Domain.Constants;
using GymX.Domain.Constants.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace GymX.Api.Common.Extensions
{
    public static class WebApplicationExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEndpointsApiExplorer();
            services.AddControllers();

            services.AddLocalization();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "vi", "en", "en-US" };
                options.SetDefaultCulture("vi")
                       .AddSupportedCultures(supportedCultures)
                       .AddSupportedUICultures(supportedCultures);
            });

            services.AddCors(options =>
            {
                options.AddPolicy("DefaultCorsPolicy", policy =>
                {
                    policy.WithOrigins("http://localhost:3000", "http://localhost:5113", "https://localhost:3000")
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });

            var jwtSecretKey = configuration["Jwt:SecretKey"] 
                ?? throw new InvalidOperationException("Jwt:SecretKey chưa được cấu hình.");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = System.Security.Claims.ClaimTypes.Role,
                    NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyNames.RequireAdmin, policy => policy.RequireRole(RoleCode.Admin));
                options.AddPolicy(PolicyNames.RequireStaff, policy => policy.RequireRole(RoleCode.Admin, RoleCode.Receptionist, RoleCode.Trainer));
                options.AddPolicy(PolicyNames.RequireTrainer, policy => policy.RequireRole(RoleCode.Trainer));
                options.AddPolicy(PolicyNames.RequireReceptionist, policy => policy.RequireRole(RoleCode.Receptionist));
                options.AddPolicy(PolicyNames.RequireMember, policy => policy.RequireRole(RoleCode.Member));
            });

            services.AddSignalR();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "GymX Management System API",
                    Version = "v1",
                    Description = "Clean Architecture Backend API for GymX System"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Nhập 'Bearer' [khoảng trắng] rồi dán token vào. Ví dụ: 'Bearer eyJhbGciOi...'"
                });

                c.AddSecurityRequirement(document =>
                    new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                    });
            });

            return services;
        }

        public static WebApplication UseApiMiddleware(this WebApplication app)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseRequestLocalization();

            // CORS phải được đặt trước UseHttpsRedirection, UseRouting, và UseAuthentication
            app.UseCors("DefaultCorsPolicy");

            app.UseStaticFiles();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GymX Management System API v1");
                    c.DisplayRequestDuration();
                    c.DefaultModelsExpandDepth(-1);
                });
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<PaymentHub>("/hubs/payment");

            return app;
        }
    }
}
