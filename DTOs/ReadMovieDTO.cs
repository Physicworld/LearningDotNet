namespace MinimalAPIPeliculas.DTOs;

public class ReadMovieDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public bool InCinemas { get; set; }
    public DateTime LaunchDate { get; set; }
    public string? Poster { get; set; }
}