using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using MediatR;
using GymX.Application.Common.Behaviors;


namespace GymX.Application.Common
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });
            return services;
        }
    }
}
