using MinimalAPIPeliculas.Entities;

namespace MinimalAPIPeliculas.Repositories;

public interface IRepositoryGenres
{
    Task<List<Genre>> GetAll();
    Task<Genre?> GetById(int Id);
    Task<int> Create(Genre genre);
    Task<bool> Exists(int Id);
    Task Update(Genre genre);
    Task Delete(int Id);
}