using MovieTheater.Application.DTOs;

namespace MovieTheater.Web.ViewModels
{
    public class HomePageViewModel
    {
        public List<MovieMainDto> LatestMovies { get; set; } = new();
        public List<MovieMainDto> AllSessions { get; set; } = new();
    }
}
