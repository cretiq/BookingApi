namespace Booking.Helper;

public class OperationResult<T>(string message, T? result = default, OperationStatus status = OperationStatus.Success)
    : Operation(message, status)
{
    public T? Result
    {
        get
        {
            if (Status == OperationStatus.Success)
                return result;

            throw new InvalidOperationException("Cannot access");
        }
    }

    public static OperationResult<T> Success(T result) => new("Success", result);
    
    public static implicit operator OperationResult<T>(T value) => Failed<T>();
}

public class Operation(string message, OperationStatus status)
{
    public string Message { get; } = message;

    public OperationStatus Status { get; } = status;

    public static Operation Created(string message = "Created") => new(message, OperationStatus.Created);
    public static Operation NotFound(string message = "Not found") => new(message, OperationStatus.NotFound);
    public static Operation Success(string message = "Success") => new(message, OperationStatus.Success);
    public static Operation Forbidden(string message = "Forbidden") => new(message, OperationStatus.Forbidden);
    public static Operation Failed(string message = "Forbidden") => new (message, OperationStatus.Failed);

    public static OperationResult<T> Failed<T>(string message = "Failed") => new(message, default, OperationStatus.Failed);
    public static OperationResult<T> Forbidden<T>(string message = "Forbidden") => new(message, default, OperationStatus.Forbidden);
    
    public static implicit operator bool(Operation result) => result?.Status == OperationStatus.Success;
}

public enum OperationStatus
{
    Created,
    NotFound,
    Success,
    Forbidden,
    Failed
}