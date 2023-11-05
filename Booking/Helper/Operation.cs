namespace Booking.Helper;

public class Operation
{
    public Operation(bool isSuccessful, Error? error = default)
        : this(isSuccessful, !isSuccessful, error) { }

    private Operation(bool isSuccessful, bool isFailure, Error? error = default)
    {
        IsSuccessful = isSuccessful;
        IsFailure = isFailure;
        Error = error ?? Error.Unknown(ErrorCodes.Unknown);
    }

    public bool IsSuccessful { get; }
    public bool IsFailure { get; }
    public Error Error { get; }

    public static implicit operator bool(Operation result) => result?.IsSuccessful ?? false;
    public static implicit operator Operation(bool boolean) => FromBoolean(boolean);
    public static implicit operator Operation(Error error) => Failure(error);

    public static Operation FromBoolean(bool boolean) => new(boolean);
    public static Operation Failure(Error error) => new(false, error);
    public static Operation Success() => new(true);

    public static OperationResult<T> Success<T>(T value) => new(true, value);
    public static OperationResult<T> Failure<T>() => new(false);
    public static OperationResult<T> Failure<T>(Error error) => new(false, default, error);
}

public class OperationResult<T>(bool isSuccessful, T? result = default, Error? error = null) : Operation(isSuccessful, error)
{
    public T? Result
    {
        get
        {
            if (IsSuccessful) return result;

            throw new InvalidOperationException(
                $"Cannot access the {nameof(Result)} property on a failed {nameof(Operation)}");
        }
    }

    public static OperationResult<T> Success(T result) => new(true, result);
    public static implicit operator OperationResult<T>(T value) => new(true, value);
    public static implicit operator OperationResult<T>(Error error) => Failure<T>(error);
}

public class Error
{
    private Error(string? details = null, ErrorKind kind = ErrorKind.Unknown)
    {
        Kind = kind;
        Details = details;
    }

    public ErrorKind Kind { get; }
    public string? Details { get; }

    public static Error Unknown(string? details = null)
    {
        details ??= ErrorCodes.Unknown;
        return new Error(details);
    }

    public static Error NotFound(string? details = null)
    {
        details ??= ErrorCodes.NotFound;
        return new Error(details, ErrorKind.NotFound);
    }

    public static Error Validation(string? details = null)
    {
        details ??= ErrorCodes.Validation;
        return new Error(details, ErrorKind.Validation);
    }

    public static Error Conflict(string? details = null)
    {
        details ??= ErrorCodes.Conflict;
        return new Error(details, ErrorKind.Conflict);
    }

    public static Error Forbidden(string? details = null)
    {
        details ??= ErrorCodes.Conflict;
        return new Error(details, ErrorKind.Forbidden);
    }
}

public static class ErrorCodes
{
    public const string Unknown = "UNKNOWN";
    public const string Forbidden = "FORBIDDEN";
    public const string NotFound = "NOT_FOUND";
    public const string Conflict = "CONFLICT";
    public const string Validation = "VALIDATION_ERROR";
}

public enum ErrorKind
{
    Unknown,
    Forbidden,
    NotFound,
    Conflict,
    Validation
}