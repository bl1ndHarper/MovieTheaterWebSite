using MovieTheater.Application.DTOs;

namespace MovieTheater.Web.ViewModels
{
    public class HomePageViewModel
    {
        public List<MovieMainDto> LatestMovies { get; set; } = new();
        public List<MovieMainDto> MoviesByDay { get; set; } = new();
        public string SelectedDate { get; set; } = string.Empty; // dd.MM
    }
}
