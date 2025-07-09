namespace MovieTheater.Domain.DTOs;
public class TmdbMovieDto
{
    public string Title { get; set; }
    public string Overview { get; set; }
    public string PosterPath { get; set; }
    public string ReleaseDate { get; set; }
    public List<TmdbGenreDto> Genres { get; set; }
    public double VoteAverage { get; set; }
    public int Runtime { get; set; }
    public bool Adult { get; set; }
}
    