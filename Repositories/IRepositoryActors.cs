using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Entities;

namespace MinimalAPIPeliculas.Repositories;

public interface IRepositoryActors
{
    Task<List<Actor>> GetAll(PaginationDTO paginationDto);
    Task<List<Actor>> GetByName(string Name);
    Task<Actor?> GetById(int Id);
    Task<int> Create(Actor actor);
    Task<bool> Exists(int Id);
    Task Update(Actor actor);
    Task Delete(int Id);
    Task<List<int>> ListExists(List<int> ids);
}