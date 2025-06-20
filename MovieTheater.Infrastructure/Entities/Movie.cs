namespace MovieTheater.Infrastructure.Entities;

public class Movie
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public short? Duration { get; set; } // in minutes
    public string? TrailerUrl { get; set; }
    public string? ThumbnailId { get; set; }
    public DateTime ReleaseDate { get; set; }
    public decimal ImdbRating { get; set; }
    public string ActivityStatus { get; set; } = null!; // Consider enum mapping
    public long AgeRatingId { get; set; }

    // Navigation
    public Rating AgeRating { get; set; } = null!;
    public ICollection<MovieGenre> Genres { get; set; } = new List<MovieGenre>();
    public ICollection<MovieActor> Actors { get; set; } = new List<MovieActor>();
}
