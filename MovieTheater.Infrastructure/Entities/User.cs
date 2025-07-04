using Microsoft.AspNetCore.Identity;

namespace MovieTheater.Infrastructure.Entities;

public class User : IdentityUser<long>
{
    public long Id { get; set; }
    public string Username { get; set; } = null!;
    public override string Email { get; set; } = null!;
    public bool IsVerified { get; set; }
    public string? VerificationToken { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
