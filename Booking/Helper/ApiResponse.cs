namespace Booking.Helper;

public class OperationResult<T>(string message, OperationStatus status) : Operation(message, status)
{
    public string Message { get; } = message;

    public OperationStatus Status { get; } = status;

    public static OperationResult<T> NotFound(string message = "Not found") => new(message, OperationStatus.NotFound);
    public static OperationResult<T> Success(string message = "Success") => new(message, OperationStatus.Success);
    public static OperationResult<T> Forbidden(string message = "Forbidden") => new(message, OperationStatus.Forbidden);
}

public class Operation(string message, OperationStatus status)
{
    public string Message { get; } = message;

    public OperationStatus Status { get; } = status;

    public static Operation NotFound(string message = "Not found") => new(message, OperationStatus.NotFound);
    public static Operation Success(string message = "Success") => new(message, OperationStatus.Success);
    public static Operation Forbidden(string message = "Forbidden") => new(message, OperationStatus.Forbidden);
}

public enum OperationStatus
{
    NotFound,
    Success,
    Forbidden
}