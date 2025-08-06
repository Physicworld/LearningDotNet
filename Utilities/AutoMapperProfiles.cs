using AutoMapper;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Entities;

namespace MinimalAPIPeliculas.Utilities;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<CreateGenreDTO, Genre>();
        CreateMap<Genre, ReadGenreDTO>();

        CreateMap<CreateActorDTO, Actor>()
            .ForMember(x => x.Photo, options => options.Ignore());
        CreateMap<Actor, ReadActorDTO>();

        CreateMap<CreateMovieDTO, Movie>()
            .ForMember(x => x.Poster, options => options.Ignore());

        CreateMap<Movie, ReadMovieDTO>()
            .ForMember(p => p.Genres,
                entity => entity.MapFrom(p =>
                    p.GenresMovies.Select(gp => new ReadGenreDTO { Id = gp.GenreId, Name = gp.Genre.Name })))
            .ForMember(p => p.Actors,
                entity => entity.MapFrom(p =>
                    p.ActorsMovies.Select(am => new ActorMovieDTO { Id = am.ActorId, Name = am.Actor.Name, Character = am.Character })));

        CreateMap<CreateCommentDTO, Comment>();
        CreateMap<Comment, ReadCommentDTO>();

        CreateMap<AssignActorMovieDTO, ActorMovie>();
    }
}