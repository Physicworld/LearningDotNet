using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Repositories;
using MinimalAPIPeliculas.Entities;
using MinimalAPIPeliculas.Filters;
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
        group.MapPost("/", Create).DisableAntiforgery().AddEndpointFilter<FilterValidations<CreateActorDTO>>();
        group.MapPut("/{id:int}", Update).DisableAntiforgery().AddEndpointFilter<FilterValidations<CreateActorDTO>>();
        group.MapDelete("/{id:int}", Delete);
        return group;
    }

    static async Task<Ok<List<ReadActorDTO>>> GetActors(IRepositoryActors repository, IMapper mapper, int page = 1,
        int recordsByPage = 10)
    {
        var pagination = new PaginationDTO
        {
            Page = page,
            RecordsByPage = recordsByPage
        };
        var actors = await repository.GetAll(pagination);
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

    static async Task<Results<Created<ReadActorDTO>, ValidationProblem>> Create(
        [FromForm] CreateActorDTO createActorDTO,
        IRepositoryActors repository,
        IOutputCacheStore outputCacheStore,
        IMapper mapper,
        IFileStorage fileStorage
    )
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


    static async Task<Results<NoContent, NotFound>> Update(
        int Id,
        [FromForm] CreateActorDTO createActorDto,
        IRepositoryActors repository,
        IFileStorage fileStorage,
        IOutputCacheStore outputCacheStore,
        IMapper mapper)
    {
        var actorDB = await repository.GetById(Id);
        if (actorDB is null)
        {
            return TypedResults.NotFound();
        }

        var actor = mapper.Map<Actor>(createActorDto);
        actor.Id = Id;
        actor.Photo = actorDB.Photo;

        if (createActorDto.Photo is not null)
        {
            var url = await fileStorage.Edit(actor.Photo, container, createActorDto.Photo);
            actor.Photo = url;
        }

        await repository.Update(actor);
        await outputCacheStore.EvictByTagAsync("actors-get", default);
        return TypedResults.NoContent();
    }

    static async Task<Results<NoContent, NotFound>> Delete(
        int Id,
        IRepositoryActors repository,
        IFileStorage fileStorage,
        IOutputCacheStore outputCacheStore
    )
    {
        var actorDB = await repository.GetById(Id);
        if (actorDB is null)
        {
            return TypedResults.NotFound();
        }

        await fileStorage.Delete(actorDB.Photo, container);
        await repository.Delete(Id);
        await outputCacheStore.EvictByTagAsync("actors-get", default);
        return TypedResults.NoContent();
    }
}