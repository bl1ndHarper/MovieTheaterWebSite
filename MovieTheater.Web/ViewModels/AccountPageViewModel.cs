using MovieTheater.Application.DTOs;

namespace MovieTheater.Web.ViewModels
{
    public class AccountPageViewModel
    {
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public List<SessionBookingsDto> Bookings { get; set; } = new();
    }
}
