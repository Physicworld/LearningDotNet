namespace MinimalAPIPeliculas.DTOs;

public class ReadActorDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime Birthday { get; set; }
    public string? Photo { get; set; }
}