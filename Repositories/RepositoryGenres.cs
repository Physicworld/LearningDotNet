using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Entities;

namespace MinimalAPIPeliculas.Repositories;

public class RepositoryGenres : IRepositoryGenres
{
    private readonly ApplicationDbContext context;

    public RepositoryGenres(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<List<Genre>> GetAll()
    {
        return await context.Genres.OrderBy(x => x.Name).ToListAsync();
    }

    public async Task<Genre?> GetById(int Id)
    {
        return await context.Genres.FirstOrDefaultAsync(x => x.Id == Id);
    }

    public async Task<int> Create(Genre genre)
    {
        context.Add(genre);
        await context.SaveChangesAsync();
        return genre.Id;
    }

    public async Task<bool> Exists(int Id)
    {
        return await context.Genres.AnyAsync(x => x.Id == Id);
    }

    public async Task<bool> Exists(int id, string Name)
    {
        return await context.Genres.AnyAsync(x => x.Id != id && x.Name == Name);
    }

    public async Task<List<int>> ListExists(List<int> ids)
    {
        return await context.Genres.Where(g => ids.Contains(g.Id)).Select(g => g.Id).ToListAsync();
    }

    public async Task Update(ReadGenreDTO genre)
    {
        context.Update(genre);
        await context.SaveChangesAsync();
    }

    public async Task Delete(int Id)
    {
        await context.Genres.Where(x => x.Id == Id).ExecuteDeleteAsync();
    }
}