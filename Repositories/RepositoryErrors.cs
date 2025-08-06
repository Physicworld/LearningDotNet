using MinimalAPIPeliculas.Entities;

namespace MinimalAPIPeliculas.Repositories;

public class RepositoryErrors : IRepositoryErrors
{
    private readonly ApplicationDbContext _context;

    public RepositoryErrors(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Create(Error error)
    {
        _context.Add(error);
        await _context.SaveChangesAsync();
    }
}