using MovieTheater.Infrastructure.Entities;

namespace MovieTheater.Web.ViewModels;

public class AdminPanelViewModel
{
    public IEnumerable<Hall> Halls { get; set; } = new List<Hall>();
}
