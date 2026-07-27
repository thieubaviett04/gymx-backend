using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;


namespace GymX.Application.Common.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>(ILogger<TRequest> logger)
        : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            logger.LogInformation("GymX Handling Request: {Name}", requestName);
            var timer = Stopwatch.StartNew();
            var response = await next();
            timer.Stop();
            logger.LogInformation("GymX Handled Request: {Name} in {Elapsed}ms", requestName, timer.ElapsedMilliseconds);
            return response;
        }
    }
}
