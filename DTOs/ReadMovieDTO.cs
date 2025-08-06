namespace MinimalAPIPeliculas.DTOs;

public class ReadMovieDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public bool InCinemas { get; set; }
    public DateTime LaunchDate { get; set; }
    public string? Poster { get; set; }
    public List<ReadCommentDTO> Comments { get; set; } = new List<ReadCommentDTO>();
    public List<ReadGenreDTO> Genres { get; set; } = new List<ReadGenreDTO>();
    public List<ActorMovieDTO> Actors { get; set; } = new List<ActorMovieDTO>();
}

