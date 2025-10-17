using System;
using System.Collections.Generic;

namespace WelfareTracker.Core.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int? Age { get; set; }

    public long MobileNumber { get; set; }

    public string? Gender { get; set; }

    public string? Address { get; set; }

    public int ConstituencyId { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string PasswordSalt { get; set; } = null!;

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiry { get; set; }

    public bool IsBlacklisted { get; set; }

    public bool IsActive { get; set; }

    public int RoleId { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

    public double Reputation { get; set; }

    public string? RoleName { get; set; }

    public string? ConstituencyName { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = [];

    public virtual ICollection<Complaint> ComplaintCitizens { get; set; } = [];

    public virtual ICollection<Complaint> ComplaintLeaders { get; set; } = [];

    public virtual Constituency Constituency { get; set; } = null!;

    public virtual ICollection<DailyComplaint> DailyComplaints { get; set; } = [];

    public virtual Role Role { get; set; } = null!;
}
