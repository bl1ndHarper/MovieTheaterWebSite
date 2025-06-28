using MovieTheater.Application.DTOs;

namespace MovieTheater.Web.ViewModels
{
    public class SessionPageViewModel
    {
        public MovieDto? MovieDetails { get; set; }
        public Dictionary<DateTime, List<MovieSessionDto>>? Sessions { get; set; }
        public List<string> AvailableDates { get; set; } = new(); // "dd.MM"
        public string SelectedDate { get; set; } = DateTime.Now.ToLocalTime().ToString("dd.MM"); // "dd.MM"
    }
}
