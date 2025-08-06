using MinimalAPIPeliculas.Entities;

namespace MinimalAPIPeliculas.Repositories;

public interface IRepositoryErrors
{
    Task Create(Error error);
}