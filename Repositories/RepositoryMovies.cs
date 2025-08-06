using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Entities;
using MinimalAPIPeliculas.Utilities;

namespace MinimalAPIPeliculas.Repositories;

public class RepositoryMovies : IRepositoryMovies
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly HttpContext _httpContext;

    public RepositoryMovies(
        ApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper
    )
    {
        _context = context;
        _httpContext = httpContextAccessor.HttpContext!;
        _mapper = mapper;
    }

    public async Task<List<Movie>> GetAll(PaginationDTO paginationDto)
    {
        var queryable = _context.Movies.AsQueryable();
        await _httpContext.InsertParametersPaginationHeader(queryable);
        return await queryable.OrderBy(p => p.Title).Page(paginationDto).ToListAsync();
    }

    public async Task<Movie?> GetById(int Id)
    {
        return await _context.Movies
            .Include(p => p.Comments)
            .Include(p => p.GenresMovies)
            .ThenInclude(p => p.Genre)
            .Include(p => p.ActorsMovies.OrderBy(a => a.Order))
            .ThenInclude(p => p.Actor)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == Id);
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
        return await _context.Movies.AnyAsync(x => x.Id == Id);
    }

    public async Task AssignGenre(int Id, List<int> genresIds)
    {
        var movie = await _context.Movies
            .Include(x => x.GenresMovies)
            .FirstOrDefaultAsync(x => x.Id == Id);
        if (movie is null)
        {
            throw new Exception($"Movie not found {Id}");
        }

        var genresMovies = genresIds.Select(genreId => new GenreMovie() { GenreId = genreId });
        movie.GenresMovies = _mapper.Map(genresMovies, movie.GenresMovies);
        await _context.SaveChangesAsync();
    }

    public async Task AssignActors(int Id, List<ActorMovie> actors)
    {
        for (int i = 1; i <= actors.Count; i++)
        {
            actors[i - 1].Order = i;
        }

        var movie = await _context.Movies
            .Include(x => x.ActorsMovies)
            .FirstOrDefaultAsync(x => x.Id == Id);
        if (movie is null)
        {
            throw new ArgumentException($"Movie not found {Id}");
        }

        movie.ActorsMovies = _mapper.Map(actors, movie.ActorsMovies);
        await _context.SaveChangesAsync();
    }
}