﻿namespace JwtAuthentication.Core.Models;

public partial class RefreshToken
{
    public int Id { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpireDate { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
