using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Entities;
using MinimalAPIPeliculas.Utilities;

namespace MinimalAPIPeliculas.Repositories;

public class RepositoryActors : IRepositoryActors
{
    private readonly ApplicationDbContext context;
    private readonly HttpContext httpContext;

    public RepositoryActors(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        this.context = context;
        httpContext = httpContextAccessor.HttpContext!;
    }

    public async Task<List<Actor>> GetAll(PaginationDTO paginationDto)
    {
        var queryable = context.Actors.AsQueryable();
        await httpContext.InsertParametersPaginationHeader(queryable);
        return await queryable.OrderBy(x => x.Name).Page(paginationDto).ToListAsync();
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

    public async Task<List<int>> ListExists(List<int> ids)
    {
        return await context.Actors.Where(x => ids.Contains(x.Id)).Select(x => x.Id).ToListAsync();
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