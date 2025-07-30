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
        CreateMap<Movie, ReadMovieDTO>();

        CreateMap<CreateCommentDTO, Comment>();
        CreateMap<Comment, ReadCommentDTO>();
    }
}