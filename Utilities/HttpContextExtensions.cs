using Microsoft.EntityFrameworkCore;

namespace MinimalAPIPeliculas.Utilities;

public static class HttpContextExtensions
{
    public async static Task InsertParametersPaginationHeader<T>(this HttpContext httpContext, IQueryable<T> queryable)
    {
        if (httpContext is null)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }

        double quantity = await queryable.CountAsync();
        httpContext.Response.Headers.Append("totalQuantityRegistries", quantity.ToString());
    }
}