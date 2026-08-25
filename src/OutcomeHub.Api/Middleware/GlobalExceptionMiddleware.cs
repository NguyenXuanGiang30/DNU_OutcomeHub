using System.Net;
using System.Text.Json;
using Npgsql;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;

namespace OutcomeHub.Api.Middleware;

public sealed partial class GlobalExceptionMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;
        HttpStatusCode statusCode;
        string message;
        IReadOnlyList<string>? errors = null;

        switch (exception)
        {
            case ValidationException validationException:
                statusCode = HttpStatusCode.BadRequest;
                message = validationException.Message;
                errors = validationException.Errors
                    .SelectMany(kvp => kvp.Value.Select(err => $"{kvp.Key}: {err}"))
                    .ToList();
                LogValidationFailure(_logger, message);
                break;

            case NotFoundException notFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = notFoundException.Message;
                LogResourceNotFound(_logger, message);
                break;

            case ForbiddenException forbiddenException:
                statusCode = HttpStatusCode.Forbidden;
                message = forbiddenException.Message;
                LogAccessForbidden(_logger, message);
                break;

            case UnauthorizedAccessException unauthorizedException:
                statusCode = HttpStatusCode.Unauthorized;
                message = unauthorizedException.Message;
                LogUnauthorizedAccess(_logger, message);
                break;

            case BusinessInvariantException invariantException:
                statusCode = HttpStatusCode.UnprocessableEntity;
                message = invariantException.Message;
                LogBusinessInvariantViolated(_logger, message);
                break;

            case PostgresException postgresException when postgresException.SqlState == "42501":
                statusCode = HttpStatusCode.Forbidden;
                message = "Access denied by database Row-Level Security policy.";
                LogRlsPolicyDenied(_logger, postgresException);
                break;

            case PostgresException postgresException:
                statusCode = HttpStatusCode.BadRequest;
                message = $"Database constraint error: {postgresException.MessageText}";
                LogPostgresError(_logger, postgresException.SqlState, postgresException);
                break;

            default:
                statusCode = HttpStatusCode.InternalServerError;
                message = _environment.IsDevelopment()
                    ? $"An internal error occurred: {exception.Message}"
                    : "An unexpected error occurred. Please contact system administrator.";
                LogUnhandledException(_logger, exception.Message, exception);
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new ApiResponse<object>
        {
            Success = false,
            Message = message,
            Errors = errors,
            TraceId = traceId,
            Timestamp = DateTime.UtcNow,
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }

    [LoggerMessage(EventId = 1001, Level = LogLevel.Warning, Message = "Validation failure: {Message}")]
    private static partial void LogValidationFailure(ILogger logger, string message);

    [LoggerMessage(EventId = 1002, Level = LogLevel.Warning, Message = "Resource not found: {Message}")]
    private static partial void LogResourceNotFound(ILogger logger, string message);

    [LoggerMessage(EventId = 1003, Level = LogLevel.Warning, Message = "Access forbidden: {Message}")]
    private static partial void LogAccessForbidden(ILogger logger, string message);

    [LoggerMessage(EventId = 1004, Level = LogLevel.Warning, Message = "Unauthorized access: {Message}")]
    private static partial void LogUnauthorizedAccess(ILogger logger, string message);

    [LoggerMessage(EventId = 1005, Level = LogLevel.Warning, Message = "Business invariant violated: {Message}")]
    private static partial void LogBusinessInvariantViolated(ILogger logger, string message);

    [LoggerMessage(EventId = 1006, Level = LogLevel.Warning, Message = "RLS policy denied query execution.")]
    private static partial void LogRlsPolicyDenied(ILogger logger, Exception exception);

    [LoggerMessage(EventId = 1007, Level = LogLevel.Error, Message = "PostgreSQL error occurred (SQLState: {SqlState})")]
    private static partial void LogPostgresError(ILogger logger, string? sqlState, Exception exception);

    [LoggerMessage(EventId = 1008, Level = LogLevel.Error, Message = "Unhandled exception occurred: {Message}")]
    private static partial void LogUnhandledException(ILogger logger, string message, Exception exception);
}
