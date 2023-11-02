using Business.Helpers;

namespace Business;

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

    public static Operation FromBoolean(bool boolean) => new Operation(boolean);

    public static Operation Failure(Error error) => new Operation(false, error);

    public static OperationResult<T> Failure<T>() => new OperationResult<T>(false, default);

    public static OperationResult<T> Failure<T>(Error error) => new OperationResult<T>(false, default, error);

    public static Operation Success() => new Operation(true);

    public static OperationResult<T> Success<T>(T value) => new OperationResult<T>(true, value);

}
