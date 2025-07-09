using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MovieTheater.Infrastructure.Enums;

namespace MovieTheater.Infrastructure.Entities;

public class Movie
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public short? Duration { get; set; }
    public string? TrailerUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public DateTime ReleaseDate { get; set; }
    public decimal ImdbRating { get; set; }
    public ActivityStatus ActivityStatus { get; set; }
    public long AgeRatingId { get; set; }
    public string DirectorName { get; set; } = null!;
    public string? DirectorDetailsUrl { get; set; }

    public Rating AgeRating { get; set; } = null!;
    public ICollection<MovieGenre> Genres { get; set; } = new List<MovieGenre>();
    public ICollection<MovieActor> Actors { get; set; } = new List<MovieActor>();
}
