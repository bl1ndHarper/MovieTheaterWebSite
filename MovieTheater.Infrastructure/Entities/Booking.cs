namespace MovieTheater.Infrastructure.Entities;

public class Booking
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long ScreeningId { get; set; }
    public long SeatId { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
    public Session Screening { get; set; } = null!;
    public HallSeat Seat { get; set; } = null!;
}
