namespace MovieTheater.Infrastructure.Entities;

public class HallSeat
{
    public long Id { get; set; }
    public long SectorId { get; set; }
    public string Label { get; set; } = null!;

    public HallSector Sector { get; set; } = null!;
}
