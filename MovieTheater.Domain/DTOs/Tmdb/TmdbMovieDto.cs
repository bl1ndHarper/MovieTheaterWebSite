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
    
    public TmdbCreditsDto Credits { get; set; }
}
    
public class TmdbCreditsDto
{
    public List<TmdbCastMemberDto> Cast { get; set; }
    public List<TmdbCrewMemberDto> Crew { get; set; }
}
public class TmdbCrewMemberDto
{
    public string Name { get; set; }
    public string Job { get; set; }
}
public class TmdbCastMemberDto
{
    public string Name { get; set; }
}