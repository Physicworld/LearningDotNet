using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Repositories;
using MinimalAPIPeliculas.Entities;

namespace MinimalAPIPeliculas.Endpoints;

public static class CommentsEndpoints
{
    public static RouteGroupBuilder MapComments(this RouteGroupBuilder group)
    {
        group.MapPost("/", Create);
        group.MapGet("/", GetComments).CacheOutput(x=>x.Expire(TimeSpan.FromSeconds(60)).Tag("comments-get"));
        group.MapGet("/{commentId:int}", GetCommentById);
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

    static async Task<Results<Created<ReadCommentDTO>, NotFound>> Create(
        int movieId,
        CreateCommentDTO createCommentDto,
        IRepositoryComments repositoryComments,
        IRepositoryMovies repositoryMovies,
        IMapper mapper,
        IOutputCacheStore outputCacheStore
    )
    {
        if (!await repositoryMovies.Exists(movieId))
        {
            return TypedResults.NotFound();
        }

        var comment = mapper.Map<Comment>(createCommentDto);
        comment.MovieId = movieId;
        var id = await repositoryComments.Create(comment);
        await outputCacheStore.EvictByTagAsync("comments-get", default);
        var readCommentDto = mapper.Map<ReadCommentDTO>(comment);
        return TypedResults.Created($"/comment/{id}", readCommentDto);
    }
}