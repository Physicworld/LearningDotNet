namespace MinimalAPIPeliculas.DTOs;

public class CreateMovieDTO
{
    public string Title { get; set; } = null!;
    public bool InCinemas { get; set; }
    public DateTime LaunchDate { get; set; }
    public IFormFile? Poster { get; set; }
}