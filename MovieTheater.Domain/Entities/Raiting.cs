namespace MovieTheater.Infrastructure.Entities;

public class Rating
{
    public long Id { get; set; }
    public string Label { get; set; } = null!;
    public string? Description { get; set; }
    public short MinAge { get; set; }

    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
