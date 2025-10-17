using System;
using System.Collections.Generic;

namespace WelfareTracker.Core.Models;

public partial class Complaint
{
    public int ComplaintId { get; set; }

    public string Description { get; set; } = null!;

    public int ConstituencyId { get; set; }

    public int CitizenId { get; set; }

    public int LeaderId { get; set; }

    public int Attempts { get; set; }

    public bool IsClosed { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

    public virtual User Citizen { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = [];

    public virtual ICollection<ComplaintImage> ComplaintImages { get; set; } = [];

    public virtual ICollection<ComplaintStatus> ComplaintStatuses { get; set; } = [];

    public virtual Constituency Constituency { get; set; } = null!;

    public virtual User Leader { get; set; } = null!;
}
