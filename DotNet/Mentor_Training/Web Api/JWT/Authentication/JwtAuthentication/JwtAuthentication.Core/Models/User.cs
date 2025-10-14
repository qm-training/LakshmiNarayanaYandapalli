namespace JwtAuthentication.Core.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Email { get; set; }

    public int? RoleId { get; set; }

    public virtual Role? Role { get; set; }
}
