using MovieTheater.Infrastructure.Entities;

public class MoviePreviewDto
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public short? Duration { get; set; }
    public string? ThumbnailUrl { get; set; }
    public DateTime ReleaseDate { get; set; }
    public decimal ImdbRating { get; set; }
    public string AgeRatingLabel { get; set; } = null!;
    public short MinAgeRating { get; set; }
    public int ReleaseYear { get; set; }
    public Genre? MainGenre { get; set; }
    public string DirectorName { get; set; } = null!;
    public string? DirectorDetailsUrl { get; set; }
    public ICollection<MovieGenre> Genres { get; set; } = new List<MovieGenre>();
    public List<string> ActorNames { get; set; } = new();
}
