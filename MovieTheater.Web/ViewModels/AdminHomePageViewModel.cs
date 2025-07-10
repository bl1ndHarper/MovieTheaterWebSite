using MovieTheater.Application.DTOs;
using MovieTheater.Infrastructure.Entities;

namespace MovieTheater.Web.ViewModels
{
    public class AdminHomePageViewModel
    {

        public List<string> Genres { get; set; } = new();
        public List<string> Ratings { get; set; } = new();

        public List<MovieMainDto> MoviesByDay { get; set; } = new();

         public List<MovieSessionsByDateDto> MoviesGroupedByDate { get; set; } = new();
        public string SelectedDate { get; set; } = string.Empty; // dd.MM
        
        public IEnumerable<Hall> Halls { get; set; } = new List<Hall>();

    }
}
