using MovieTheater.Application.DTOs;

namespace MovieTheater.Web.ViewModels
{
    public class UpcomingPageViewModel
    {
        public List<TmdbUpcomingMovie> UpcomingMovies { get; set; } = new();

    }
}
