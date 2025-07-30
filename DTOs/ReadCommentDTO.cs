namespace MinimalAPIPeliculas.DTOs;

public class ReadCommentDTO
{
    public int Id { get; set; }
    public string Body { get; set; }
    public int MovieId { get; set; }
}