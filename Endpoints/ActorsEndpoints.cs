using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Repositories;
using MinimalAPIPeliculas.Entities;
using MinimalAPIPeliculas.Services;

namespace MinimalAPIPeliculas.Endpoints;

public static class ActorsEndpoints
{
    private static readonly string container = "actors";

    public static RouteGroupBuilder MapActors(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetActors).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("actors-get"));
        group.MapGet("/{id:int}", GetByID);
        group.MapGet("getByName/{name}", GetByName);
        group.MapPost("/", Create).DisableAntiforgery();
        return group;
    }

    static async Task<Ok<List<ReadActorDTO>>> GetActors(IRepositoryActors repository, IMapper mapper)
    {
        var actors = await repository.GetAll();
        var actorsDTOs = mapper.Map<List<ReadActorDTO>>(actors);
        return TypedResults.Ok(actorsDTOs);
    }

    static async Task<Results<Ok<ReadActorDTO>, NotFound>> GetByID(int Id, IRepositoryActors repository, IMapper mapper)
    {
        var actor = await repository.GetById(Id);
        if (actor is null)
        {
            return TypedResults.NotFound();
        }

        var readActorDTO = mapper.Map<ReadActorDTO>(actor);
        return TypedResults.Ok(readActorDTO);
    }

    static async Task<Results<Ok<List<ReadActorDTO>>, NotFound>> GetByName(string name, 
    IRepositoryActors repository, IMapper mapper)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return TypedResults.NotFound();
        }

        var actors = await repository.GetByName(name);
        if (actors.Count == 0)
        {
            return TypedResults.NotFound();
        }
        
        var actorsDTOs = mapper.Map<List<ReadActorDTO>>(actors);
        return TypedResults.Ok(actorsDTOs);
    }

    static async Task<Created<ReadActorDTO>> Create([FromForm] CreateActorDTO createActorDTO,
        IRepositoryActors repository, IOutputCacheStore outputCacheStore, IMapper mapper, IFileStorage fileStorage)
    {
        var actor = mapper.Map<Actor>(createActorDTO);
        if (createActorDTO.Photo is not null)
        {
            var url = await fileStorage.Store(container, createActorDTO.Photo);
            actor.Photo = url;
        }

        var id = await repository.Create(actor);
        await outputCacheStore.EvictByTagAsync("actors-get", default);
        var readActorDTO = mapper.Map<ReadActorDTO>(actor);
        return TypedResults.Created($"/actors/{id}", readActorDTO);
    }
}