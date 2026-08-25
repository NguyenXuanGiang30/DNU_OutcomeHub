namespace OutcomeHub.Application.Common.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public IReadOnlyList<string>? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? TraceId { get; set; }
}

public static class ApiResponse
{
    public static ApiResponse<T> Ok<T>(T data, string? message = null) =>
        new()
        {
            Success = true,
            Data = data,
            Message = message,
        };

    public static ApiResponse<object?> Ok(string? message = null) =>
        new()
        {
            Success = true,
            Data = null,
            Message = message,
        };

    public static ApiResponse<T> Fail<T>(string message, IReadOnlyList<string>? errors = null) =>
        new()
        {
            Success = false,
            Message = message,
            Errors = errors,
        };

    public static ApiResponse<object?> Fail(string message, IReadOnlyList<string>? errors = null) =>
        new()
        {
            Success = false,
            Data = null,
            Message = message,
            Errors = errors,
        };
}
