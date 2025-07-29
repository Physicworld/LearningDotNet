namespace MinimalAPIPeliculas.DTOs;

public class CreateActorDTO
{
    public string Name { get; set; } = null!;
    public DateTime? Birthday { get; set; }
    public IFormFile? Photo { get; set; }
}