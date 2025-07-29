using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Repositories;
using MinimalAPIPeliculas.Services;
using MinimalAPIPeliculas.Entities;

namespace MinimalAPIPeliculas.Endpoints;

public static class MoviesEndpoints
{
    private static readonly string container = "movies";

    public static RouteGroupBuilder MapMovies(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetMovies).CacheOutput(x => x.Expire(TimeSpan.FromSeconds(60)).Tag("movies-get"));
        group.MapGet("/{Id:int}", GetByID);
        group.MapPost("/", Create).DisableAntiforgery();
        return group;
    }

    static async Task<Ok<List<ReadMovieDTO>>> GetMovies(
        IRepositoryMovies repository,
        IMapper mapper,
        int page = 1,
        int recordsByPage = 10)
    {
        var pagination = new PaginationDTO
        {
            Page = page,
            RecordsByPage = recordsByPage
        };
        var movies = await repository.GetAll(pagination);
        var moviesDTO = mapper.Map<List<ReadMovieDTO>>(movies);
        return TypedResults.Ok(moviesDTO);
    }

    static async Task<Results<Ok<ReadMovieDTO>, NotFound>> GetByID(int Id, IRepositoryMovies repository, IMapper mapper)
    {
        var movie = await repository.GetById(Id);
        if (movie is null)
        {
            return TypedResults.NotFound();
        }

        var readMovieDTO = mapper.Map<ReadMovieDTO>(movie);
        return TypedResults.Ok(readMovieDTO);
    }

    static async Task<Created<ReadMovieDTO>> Create(
        [FromForm] CreateMovieDTO createMovieDTO,
        IRepositoryMovies repository,
        IFileStorage fileStorage,
        IOutputCacheStore outputCacheStore,
        IMapper mapper)
    {
        var movie = mapper.Map<Movie>(createMovieDTO);

        if (createMovieDTO.Poster is not null)
        {
            var url = await fileStorage.Store(container, createMovieDTO.Poster);
            movie.Poster = url;
        }

        var Id = await repository.Create(movie);
        await outputCacheStore.EvictByTagAsync("movies-get", default);
        var readMovieDTO = mapper.Map<ReadMovieDTO>(movie);
        return TypedResults.Created($"/movies/{Id}", readMovieDTO);
    }

    static async Task<Results<NoContent, NotFound>> Update(
        int Id,
        [FromForm] CreateMovieDTO createMovieDTO,
        IRepositoryMovies repository,
        IFileStorage fileStorage,
        IOutputCacheStore outputCacheStore,
        IMapper mapper)
    {
        var movieDB = await repository.GetById(Id);
        if (movieDB is null)
        {
            return TypedResults.NotFound();
        }

        var movie = mapper.Map<Movie>(createMovieDTO);
        movie.Id = Id;
        movie.Poster = movieDB.Poster;

        if (createMovieDTO.Poster is not null)
        {
            var url = await fileStorage.Edit(movie.Poster, container, createMovieDTO.Poster);
            movie.Poster = url;
        }
        await repository.Update(movie);
        await outputCacheStore.EvictByTagAsync("movies-get", default);
        return TypedResults.NoContent();
    }
    
    static async Task<Results<NoContent, NotFound>> Delete(
        int Id,
        IRepositoryMovies repository,
        IFileStorage fileStorage,
        IOutputCacheStore outputCacheStore)
    {
        var movieDB = await repository.GetById(Id);
        if (movieDB is null)
        {
            return TypedResults.NotFound();
        }
        await fileStorage.Delete(movieDB.Poster, container);
        await repository.Delete(Id);
        await outputCacheStore.EvictByTagAsync("movies-get", default);
        return TypedResults.NoContent();
    }
}