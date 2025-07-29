using MinimalAPIPeliculas.DTOs;

namespace MinimalAPIPeliculas.Utilities;

public static class IQueryableExtensions
{
    public static IQueryable<T> Page<T>(this IQueryable<T> queryable, PaginationDTO paginationDto)
    {
        return queryable.Skip((paginationDto.Page - 1) * paginationDto.RecordsByPage).Take(paginationDto.RecordsByPage);
    }
}