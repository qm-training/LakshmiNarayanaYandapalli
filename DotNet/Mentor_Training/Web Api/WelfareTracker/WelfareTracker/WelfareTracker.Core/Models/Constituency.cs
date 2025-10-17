using System;
using System.Collections.Generic;

namespace WelfareTracker.Core.Models;

public partial class Constituency
{
    public int ConstituencyId { get; set; }

    public string ConstituencyName { get; set; } = null!;

    public string DistrictName { get; set; } = null!;

    public string StateName { get; set; } = null!;

    public string CountryName { get; set; } = null!;

    public long Pincode { get; set; }

    public virtual ICollection<Complaint> Complaints { get; set; } = [];

    public virtual ICollection<DailyComplaint> DailyComplaints { get; set; } = [];

    public virtual ICollection<User> Users { get; set; } = [];
}
