namespace MinimalAPIPeliculas.Entities;

public class Actor
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime Birthday { get; set; }
    public string? Photo { get; set; }
    public List<ActorMovie> ActorsMovies { get; set; } = new List<ActorMovie>();
}