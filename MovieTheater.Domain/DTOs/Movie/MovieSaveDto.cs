public class MovieSaveDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public short? Duration { get; set; }
    public string ThumbnailUrl { get; set; } = null!;
    public DateTime ReleaseDate { get; set; }
    public decimal ImdbRating { get; set; }
    public string DirectorName { get; set; } = "";
    public string? DirectorDetailsUrl { get; set; }
    public bool Adult { get; set; }

    public List<string> Genres { get; set; } = new();
}
