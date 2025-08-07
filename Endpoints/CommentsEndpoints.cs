using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Repositories;
using MinimalAPIPeliculas.Entities;
using MinimalAPIPeliculas.Filters;
using MinimalAPIPeliculas.Services;

namespace MinimalAPIPeliculas.Endpoints;

public static class CommentsEndpoints
{
    public static RouteGroupBuilder MapComments(this RouteGroupBuilder group)
    {
        group.MapPost("/", Create)
            .AddEndpointFilter<FilterValidations<CreateCommentDTO>>()
            .RequireAuthorization();

        group.MapGet("/", GetComments)
            .CacheOutput(x => x.Expire(TimeSpan.FromSeconds(60)).Tag("comments-get"));

        group.MapGet("/{commentId:int}", GetCommentById);

        group.MapPut("/{commentId:int}", Update)
            .AddEndpointFilter<FilterValidations<CreateCommentDTO>>()
            .RequireAuthorization();

        group.MapDelete("/{commentId:int}", Delete)
            .RequireAuthorization();
        return group;
    }

    static async Task<Results<Ok<List<ReadCommentDTO>>, NotFound>> GetComments(
        int movieId,
        IRepositoryComments repositoryComments,
        IRepositoryMovies repositoryMovies,
        IMapper mapper
    )
    {
        if (!await repositoryMovies.Exists(movieId))
        {
            return TypedResults.NotFound();
        }

        var comments = await repositoryComments.GetAll(movieId);
        var readCommentDtos = mapper.Map<List<ReadCommentDTO>>(comments);
        return TypedResults.Ok(readCommentDtos);
    }

    static async Task<Results<Ok<ReadCommentDTO>, NotFound>> GetCommentById(
        int movieId,
        int commentId,
        IRepositoryComments repositoryComments,
        IRepositoryMovies repositoryMovies,
        IMapper mapper
    )
    {
        if (!await repositoryMovies.Exists(movieId))
        {
            return TypedResults.NotFound();
        }

        var comment = await repositoryComments.GetById(commentId);
        if (comment is null)
        {
            return TypedResults.NotFound();
        }

        if (comment.MovieId != movieId)
        {
            return TypedResults.NotFound();
        }

        var readCommentDto = mapper.Map<ReadCommentDTO>(comment);
        return TypedResults.Ok(readCommentDto);
    }

    static async Task<Results<Created<ReadCommentDTO>, NotFound, BadRequest<string>>> Create(
        int movieId,
        CreateCommentDTO createCommentDto,
        IRepositoryComments repositoryComments,
        IRepositoryMovies repositoryMovies,
        IMapper mapper,
        IOutputCacheStore outputCacheStore,
        IUserService userService
    )
    {
        if (!await repositoryMovies.Exists(movieId))
        {
            return TypedResults.NotFound();
        }

        var comment = mapper.Map<Comment>(createCommentDto);
        comment.MovieId = movieId;

        var user = await userService.GetUser();
        if (user is null)
        {
            return TypedResults.BadRequest("User not found");
        }

        comment.UserId = user.Id;
        var id = await repositoryComments.Create(comment);
        await outputCacheStore.EvictByTagAsync("comments-get", default);
        var readCommentDto = mapper.Map<ReadCommentDTO>(comment);
        return TypedResults.Created($"/comment/{id}", readCommentDto);
    }

    static async Task<Results<NoContent, NotFound, ForbidHttpResult>> Update(
        int movieId,
        int commentId,
        CreateCommentDTO createCommentDto,
        IRepositoryComments repositoryComments,
        IRepositoryMovies repositoryMovies,
        IOutputCacheStore outputCacheStore,
        IUserService userService
    )
    {
        if (!await repositoryMovies.Exists(movieId))
        {
            return TypedResults.NotFound();
        }

        var commentDB = await repositoryComments.GetById(commentId);
        if (commentDB is null)
        {
            return TypedResults.NotFound();
        }

        var user = await userService.GetUser();

        if (user is null)
        {
            return TypedResults.NotFound();
        }

        if (commentDB.UserId != user.Id)
        {
            return TypedResults.Forbid();
        }

        commentDB.Body = createCommentDto.Body;
        await repositoryComments.Update(commentDB);
        await outputCacheStore.EvictByTagAsync("comments-get", default);
        return TypedResults.NoContent();
    }

    static async Task<Results<NoContent, NotFound, ForbidHttpResult>> Delete(
        int movieId,
        int commentId,
        IRepositoryComments repositoryComments,
        IOutputCacheStore outputCacheStore,
        IUserService userService
    )
    {
        var commentDB = await repositoryComments.GetById(commentId);
        if (commentDB is null)
        {
            return TypedResults.NotFound();
        }

        var user = await userService.GetUser();

        if (user is null)
        {
            return TypedResults.NotFound();
        }

        if (commentDB.UserId != user.Id)
        {
            return TypedResults.Forbid();
        }

        await repositoryComments.Delete(commentId);
        await outputCacheStore.EvictByTagAsync("comments-get", default);
        return TypedResults.NoContent();
    }
}