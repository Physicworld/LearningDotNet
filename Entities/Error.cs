namespace MinimalAPIPeliculas.Entities;

public class Error
{
    public Guid Id { get; set; }
    public string Message { get; set; } = null!;
    public string? StackTrace { get; set; }
    public DateTime Date { get; set; }
}