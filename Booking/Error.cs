namespace Business.Helpers;

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
}
