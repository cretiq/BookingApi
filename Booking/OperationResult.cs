using Business.Helpers;

namespace Business;

public class OperationResult<T> : Operation
{
    private readonly T? _result;

    public OperationResult(bool isSuccessful, T? result = default, Error? error = null)
        : base(isSuccessful, error)
    {
        _result = result;
    }

    public T? Result
    {
        get
        {
            if (IsSuccessful)
            {
                return _result;
            }

            throw new InvalidOperationException(
                $"Cannot access the {nameof(Result)} property on a failed {nameof(Operation)}");
        }
    }

    public static OperationResult<T> Success(T result) => new(true, result);

    public static implicit operator OperationResult<T>(T value) => new OperationResult<T>(true, value);

    public static implicit operator OperationResult<T>(Error error) => Failure<T>(error);

}
