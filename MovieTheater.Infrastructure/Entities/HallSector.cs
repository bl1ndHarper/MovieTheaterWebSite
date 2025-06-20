namespace MovieTheater.Infrastructure.Entities;

public class HallSector
{
    public long Id { get; set; }
    public long HallId { get; set; }
    public string Name { get; set; } = null!;
    public decimal SeatPrice { get; set; }

    public Hall Hall { get; set; } = null!;
    public ICollection<HallSeat> Seats { get; set; } = new List<HallSeat>();
}
