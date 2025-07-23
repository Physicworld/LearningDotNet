using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas.Entities;

namespace MinimalAPIPeliculas.Repositories;

public class RepositoryActors : IRepositoryActors
{
    private readonly ApplicationDbContext context;

    public RepositoryActors(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<List<Actor>> GetAll()
    {
        return await context.Actors.OrderBy(x => x.Name).ToListAsync();
    }

    public async Task<Actor> GetById(int Id)
    {
        return await context.Actors.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id);
    }

    public async Task<List<Actor>> GetByName(string Name)
    {
        return await context.Actors.Where(x => x.Name.Contains(Name)).OrderBy(x => x.Name).ToListAsync();
    }


    public async Task<int> Create(Actor actor)
    {
        context.Add(actor);
        await context.SaveChangesAsync();
        return actor.Id;
    }

    public async Task<bool> Exists(int Id)
    {
        return await context.Actors.AnyAsync(x => x.Id == Id);
    }

    public async Task Update(Actor actor)
    {
        context.Update(actor);
        await context.SaveChangesAsync();
    }

    public async Task Delete(int Id)
    {
        await context.Actors.Where(x => x.Id == Id).ExecuteDeleteAsync();
    }
}