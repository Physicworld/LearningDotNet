using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Entities;

namespace MinimalAPIPeliculas.Repositories;

public interface IRepositoryGenres
{
    Task<List<Genre>> GetAll();
    Task<Genre?> GetById(int Id);
    Task<int> Create(Genre genre);
    Task<bool> Exists(int Id);
    Task Update(ReadGenreDTO genre);
    Task Delete(int Id);
    Task<List<int>> ListExists(List<int> ids);
    Task<bool> Exists(int id, string Name);
}