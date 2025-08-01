using System.ComponentModel.DataAnnotations;

namespace MinimalAPIPeliculas.Entities;

public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<GenreMovie> GenresMovies { get; set; } = new List<GenreMovie>();
}