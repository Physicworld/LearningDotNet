namespace MinimalAPIPeliculas.Entities;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public bool InCinemas { get; set; }
    public DateTime LaunchDate { get; set; }
    public string? Poster { get; set; }
    public List<Comment> Comments { get; set; } = new List<Comment>();
    public List<GenreMovie> GenresMovies { get; set; } = new List<GenreMovie>();
    public List<ActorMovie> ActorsMovies { get; set; } = new List<ActorMovie>();
} 