namespace MovieTheater.Infrastructure.Entities;

public class Hall
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public ICollection<HallSector> Sectors { get; set; } = new List<HallSector>();
}
