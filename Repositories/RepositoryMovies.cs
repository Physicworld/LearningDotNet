using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Entities;
using MinimalAPIPeliculas.Utilities;

namespace MinimalAPIPeliculas.Repositories;

public class RepositoryMovies : IRepositoryMovies
{
    private readonly ApplicationDbContext _context;
    private readonly HttpContext _httpContext;

    public RepositoryMovies(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContext = httpContextAccessor.HttpContext!;
    }

    public async Task<List<Movie>> GetAll(PaginationDTO paginationDto)
    {
        var queryable = _context.Movies.AsQueryable();
        await _httpContext.InsertParametersPaginationHeader(queryable);
        return await queryable.OrderBy(p => p.Title).Page(paginationDto).ToListAsync();
    }

    public async Task<Movie?> GetById(int Id)
    {
        return await _context.Movies.AsNoTracking().FirstOrDefaultAsync(p => p.Id == Id);
    }

    public async Task<int> Create(Movie movie)
    {
        _context.Add(movie);
        await _context.SaveChangesAsync();
        return movie.Id;
    }

    public async Task Update(Movie movie)
    {
        _context.Update(movie);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int Id)
    {
        await _context.Movies.Where(x => x.Id == Id).ExecuteDeleteAsync();
    }

    public async Task<bool> Exists(int Id)
    {
        return await _context.Actors.AnyAsync(x => x.Id == Id);
    }
}