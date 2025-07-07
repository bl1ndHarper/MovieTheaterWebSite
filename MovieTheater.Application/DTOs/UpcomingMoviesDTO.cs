using System.Text.Json.Serialization;
using MovieTheater.Infrastructure.Entities;

public class TmdbUpcomingMoviesResponse
{
    public List<TmdbUpcomingMovie> Results { get; set; } = new();
}

public class TmdbUpcomingMovie
{
    public string? Title { get; set; }

    [JsonPropertyName("release_date")]
    public DateTime? ReleaseDate { get; set; }

    public string? Overview { get; set; }

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; set; }

    [JsonPropertyName("genre_ids")]
    public List<int> GenreIds { get; set; } 
    public string GenreNames { get; set; } = string.Empty;
}

public class TmdbGenre
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class TmdbGenreResponse
{
    public List<TmdbGenre> Genres { get; set; } = new();
}