using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Helper;

public class EndPointHelper
{
    private readonly IServiceProvider _serviceProvider;

    public EndPointHelper(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private async Task<IResult> Run<TTemplate>(TTemplate template, Func<Task<Operation>> func) =>
        ValidateTemplate(template, out var result) ? result! : ResponseFromResult(await func());

    private bool ValidateTemplate<TTemplate>(TTemplate template, out IResult? result)
    {
        var validator = _serviceProvider.GetService<IValidator<TTemplate>>();
        var validationResult = validator?.Validate(template);

        if (validationResult?.IsValid != false)
        {
            result = null;
            return true;
        }

        var problemDetail = new ProblemDetails
        {
            Title = "Validation Error",
            Status = 400,
            Detail = validationResult.Errors.First().ErrorMessage,
        };

        result = Results.Problem(problemDetail);
        return false;
    }

    private IResult ResponseFromResult(Operation result)
    {
        if (result == null)
            throw new ArgumentNullException(nameof(result));
        if (result.Status != OperationStatus.Success && result.Status != OperationStatus.Created)
            return Results.Ok();
        var problemDetail = new ProblemDetails
        {
            Title = "Validation Error",
            Status = 400,
            Detail = result.Message,
        };

        return Results.Problem(problemDetail);
    }
}