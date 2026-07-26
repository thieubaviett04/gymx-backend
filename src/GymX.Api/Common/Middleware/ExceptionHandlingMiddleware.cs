using GymX.Application.Common.Exceptions;
using GymX.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GymX.Api.Common.Middleware
{
    public class ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception occurred while processing request: {Path}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var (statusCode, problemDetails) = exception switch
            {
                ValidationException validationEx => (
                    StatusCodes.Status400BadRequest,
                    new ValidationProblemDetails(validationEx.Errors)
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = ErrorCodes.Common.ValidationError,
                        Detail = "One or more validation errors occurred.",
                        Instance = context.Request.Path
                    } as ProblemDetails
                ),
                NotFoundException notFoundEx => (
                    StatusCodes.Status404NotFound,
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = ErrorCodes.Common.NotFound,
                        Detail = notFoundEx.Message,
                        Instance = context.Request.Path
                    }
                ),
                ForbiddenAccessException => (
                    StatusCodes.Status403Forbidden,
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Title = ErrorCodes.Common.Forbidden,
                        Detail = "You do not have permission to access this resource.",
                        Instance = context.Request.Path
                    }
                ),
                UnauthorizedAccessException => (
                    StatusCodes.Status401Unauthorized,
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Title = ErrorCodes.Common.Unauthorized,
                        Detail = "Authentication credentials are missing or invalid.",
                        Instance = context.Request.Path
                    }
                ),
                _ => (
                    StatusCodes.Status500InternalServerError,
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = ErrorCodes.Common.InternalServerError,
                        Detail = environment.IsDevelopment() ? exception.Message : "An unexpected server error occurred.",
                        Instance = context.Request.Path
                    }
                )
            };
            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, problemDetails.GetType(), JsonOptions));
        }
    }
}
