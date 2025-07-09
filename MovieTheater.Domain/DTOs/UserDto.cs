public class UserDto
{
    public long Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsVerified { get; set; }
    public bool IsAdmin { get; set; }
}
