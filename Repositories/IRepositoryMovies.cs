using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Entities;

namespace MinimalAPIPeliculas.Repositories;

public interface IRepositoryMovies
{
    Task<List<Movie>> GetAll(PaginationDTO paginationDto);
    Task<Movie?> GetById(int Id);
    Task<int> Create(Movie movie);
    Task Update(Movie movie);
    Task Delete(int Id);
    Task<bool> Exists(int Id);
}