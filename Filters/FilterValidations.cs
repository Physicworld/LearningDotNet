using FluentValidation;
using MinimalAPIPeliculas.DTOs;

namespace MinimalAPIPeliculas.Filters;

public class FilterValidations<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
        if (validator is null)
        {
            return await next(context);
        }

        var inputValidate = context.Arguments.OfType<T>().FirstOrDefault();
        if (inputValidate is null)
        {
            return TypedResults.Problem("Entity for validation not found!");
        }

        var validationResult = await validator.ValidateAsync(inputValidate);
        if (!validationResult.IsValid)
        {
            return TypedResults.UnprocessableEntity(validationResult.Errors);
        }

        return await next(context);
    }
}