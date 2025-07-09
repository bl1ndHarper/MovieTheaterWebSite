public class MovieDetailsDto
{
    public string Title { get; set; }
    public string Overview { get; set; }
    public string PosterPath { get; set; }
    public decimal VoteAverage { get; set; }
    public bool Adult { get; set; }
    public short? Runtime { get; set; }
    public string ReleaseDate { get; set; }
    public List<GenreDto> Genres { get; set; }
}

public class GenreDto
{
    public string Name { get; set; }
}
