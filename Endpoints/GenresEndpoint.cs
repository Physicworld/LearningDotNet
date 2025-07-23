using AutoMapper;
using MinimalAPIPeliculas.Entities;
using MinimalAPIPeliculas.Repositories;

using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.Http.HttpResults;
using MinimalAPIPeliculas.DTOs;

namespace MinimalAPIPeliculas.Endpoints;

public static class GenresEndpoint
{
    public static RouteGroupBuilder MapGenres(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetGenres)
            .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("genres-get"));
        group.MapGet("/{id:int}", GetGenreById);
        group.MapPost("/", CreateGenre);
        group.MapPut("/{id:int}", UpdateGenre);
        group.MapDelete("/{id:int}", DeleteGenre);
        return group;
    }
    static async Task<Ok<List<ReadGenreDTO>>> GetGenres(IRepositoryGenres repository, IMapper mapper)
    {
        var genres = await repository.GetAll();
        var readGenresDTOs = mapper.Map<List<ReadGenreDTO>>(genres);
        return TypedResults.Ok(readGenresDTOs);
    }

    static async Task<Results<Ok<ReadGenreDTO>, NotFound>> GetGenreById(IRepositoryGenres repository, IMapper mapper, int Id)
    {
        var genre = await repository.GetById(Id);
        if (genre is null)
        {
            return TypedResults.NotFound();
        }
        var readGenreDTO = mapper.Map<ReadGenreDTO>(genre);
        return TypedResults.Ok(readGenreDTO);
    }

    static async Task<Created<ReadGenreDTO>> CreateGenre(CreateGenreDTO createGenreDTO, IRepositoryGenres repository,
        IOutputCacheStore outputCacheStore, IMapper mapper)
    {
        var genre = mapper.Map<Genre>(createGenreDTO);
        var id = await repository.Create(genre);
        await outputCacheStore.EvictByTagAsync("genres-get", default);
        var readGenreDTO = mapper.Map<ReadGenreDTO>(genre);
        return TypedResults.Created($"/genres/{id}", readGenreDTO);
    }


    static async Task<Results<NoContent, NotFound>> UpdateGenre(int Id, Genre genre, IRepositoryGenres repository,
        IOutputCacheStore outputCacheStore, IMapper mapper)
    {
        var exists = await repository.Exists(Id);
        if (!exists)
        {
            return TypedResults.NotFound();
        }
        var readGenreDTO = mapper.Map<ReadGenreDTO>(genre);
        readGenreDTO.Id = Id;
        await repository.Update(genre);
        await outputCacheStore.EvictByTagAsync("genres-get", default);
        return TypedResults.NoContent();
    }


    static async Task<Results<NoContent, NotFound>> DeleteGenre(int Id, IRepositoryGenres repository,
        IOutputCacheStore outputCacheStore)
    {
        var exists = await repository.Exists(Id);
        if (!exists)
        {
            return TypedResults.NotFound();
        }

        await repository.Delete(Id);
        await outputCacheStore.EvictByTagAsync("genres-get", default);
        return TypedResults.NoContent();
    }
}