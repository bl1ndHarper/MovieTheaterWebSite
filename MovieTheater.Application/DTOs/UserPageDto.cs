using MovieTheater.Infrastructure.Enums;

public class UserPageDto
{
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
}

public class SessionBookingsDto
{
    public long ScreeningId { get; set; }
    public DateTime ScreeningTime { get; set; }
    public List<TicketDto> Tickets { get; set; } = new();

    public string MovieTitle { get; set; } = "";
    public string HallName { get; set; } = "";
}

public class TicketDto
{
    public long BookingId { get; set; }
    public string SeatNumber { get; set; }
    public BookingStatus Status { get; set; }
    
    public decimal SeatPrice { get; set; }
}