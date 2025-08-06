using AutoMapper;
using MinimalAPIPeliculas.Repositories;

namespace MinimalAPIPeliculas.Filters;

public class TestFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // This code is executed before the endpoint
        var paramRepositoryGenres = context.Arguments.OfType<IRepositoryGenres>().FirstOrDefault();
        var paramMapper = context.Arguments.OfType<IMapper>().FirstOrDefault();
        var paramInt = context.Arguments.OfType<int>().FirstOrDefault();

        var result = await next(context);
        // This code is executed after the endpoint
        return result;
    }
}