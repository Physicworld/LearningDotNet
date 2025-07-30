using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas.Entities;

namespace MinimalAPIPeliculas.Repositories;

public class RepositoryComments : IRepositoryComments
{
    private readonly ApplicationDbContext _context;

    public RepositoryComments(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Comment>> GetAll(int movieId)
    {
        return await _context.Comments.Where(x => x.MovieId == movieId).ToListAsync();
    }

    public async Task<Comment?> GetById(int commentId)
    {
        return await _context.Comments.FirstOrDefaultAsync(x => x.Id == commentId);
    }

    public async Task<int> Create(Comment comment)
    {
        _context.Add(comment);
        await _context.SaveChangesAsync();
        return comment.Id;
    }

    public async Task<bool> Exists(int Id)
    {
        return await _context.Comments.AnyAsync(x => x.Id == Id);
    }

    public async Task Update(Comment comment)
    {
        _context.Update(comment);
        await _context.SaveChangesAsync();
    }

    public async Task Delte(int Id)
    {
        await _context.Comments.Where(x => x.Id == Id).ExecuteDeleteAsync();
    }
}