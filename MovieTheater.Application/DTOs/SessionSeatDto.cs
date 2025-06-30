using MovieTheater.Infrastructure.Enums;

public class SessionSeatDto
{
    public string SectorName { get; set; } = null!;
    public string Label { get; set; } = null!; // A1, B2 тощо
    public string Row => Label.Substring(0, 1); // "A"
    public int Number => int.TryParse(Label.Substring(1), out var n) ? n : 0; // 1, 2 тощо
    public decimal Price { get; set; }
    public HallSeatStatus Status { get; set; }
}
