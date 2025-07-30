using MinimalAPIPeliculas.Entities;

namespace MinimalAPIPeliculas.Repositories;

public interface IRepositoryComments
{
    Task<List<Comment>> GetAll(int movieId);
    Task<Comment?> GetById(int commentId);
    Task<int> Create(Comment comment);
    Task<bool> Exists(int Id);
    Task Update(Comment comment);
    Task Delete(int Id);
}