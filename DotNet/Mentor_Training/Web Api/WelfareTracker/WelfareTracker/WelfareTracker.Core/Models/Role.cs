using System;
using System.Collections.Generic;

namespace WelfareTracker.Core.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = [];
}
