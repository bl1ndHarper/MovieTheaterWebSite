namespace MovieTheater.Domain.DTOs;
public class TmdbMovieDto
{
    public string Title { get; set; }
    public string Overview { get; set; }
    public string Poster_Path { get; set; }
    public string Release_Date { get; set; }
    public List<TmdbGenreDto> Genres { get; set; }
    public double Vote_Average { get; set; }
    public int Runtime { get; set; }
    public bool Adult { get; set; }
}
    