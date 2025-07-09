using MovieTheater.Infrastructure.Enums;

public class SessionSeatDto
{
    public long SessionId { get; set; }
    public string SectorName { get; set; } = null!;
    public string Label { get; set; } = null!; // A1, B2 ����
    public string Row => Label.Substring(0, 1); // "A"
    public int Number => int.TryParse(Label.Substring(1), out var n) ? n : 0; // 1, 2 ����
    public decimal Price { get; set; }
    public SeatStatus Status { get; set; }
}
