public class MovieDto
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? ThumbnailId { get; set; }
    public DateTime ReleaseDate { get; set; }
    public decimal ImdbRating { get; set; }
    public string AgeRatingLabel { get; set; } = null!;
    public IEnumerable<string> Genres { get; set; } = new List<string>();
}
