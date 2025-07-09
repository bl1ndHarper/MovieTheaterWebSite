public class BookingRequestDto
{
    public long UserId { get; set; }
    public long ScreeningId { get; set; }
    public List<long> SeatIds { get; set; } = new();
}
