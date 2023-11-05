using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Helper;

public class EndPointHelper(IServiceProvider serviceProvider) : IEndPointHelper
{
    public async Task<IResult> Run<TTemplate>(TTemplate template, Func<Task<Operation>> func) =>
        ValidateTemplate(template, out var result) ? result! : ResponseFromResult(await func());

    public async Task<IResult> Run<TTemplate, T>(TTemplate template, Func<Task<OperationResult<T>>> func) =>
        ValidateTemplate(template, out var result) ? result! : ResponseFromResult(await func());

    public async Task<IResult> Run(Func<Task<Operation>> func) => ResponseFromResult(await func());

    private bool ValidateTemplate<TTemplate>(TTemplate template, out IResult? result)
    {
        var validator = serviceProvider.GetService<IValidator<TTemplate>>();
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
            Detail = validationResult.Errors.First().ErrorMessage
        };

        result = Results.Problem(problemDetail);
        return false;
    }

    private IResult ResponseFromResult(Operation result)
    {
        if (result == null)
            throw new ArgumentNullException(nameof(result));

        if (result.IsSuccessful)
            return Results.Ok();

        var problemDetail = new ProblemDetails
        {
            Title = "Validation Error",
            Status = 400,
            Detail = result.Error.Details
        };

        return Results.Problem(problemDetail);
    }
}

public interface IEndPointHelper
{
    Task<IResult> Run(Func<Task<Operation>> func);
    Task<IResult> Run<TTemplate>(TTemplate template, Func<Task<Operation>> func);
    Task<IResult> Run<TTemplate, T>(TTemplate template, Func<Task<OperationResult<T>>> func);
}