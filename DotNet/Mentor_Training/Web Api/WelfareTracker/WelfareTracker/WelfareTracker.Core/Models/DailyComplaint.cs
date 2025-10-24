using System;
using System.Collections.Generic;

namespace WelfareTracker.Core.Models;

public partial class DailyComplaint
{
    public int DailyComplaintId { get; set; }

    public int ConstituencyId { get; set; }

    public int LeaderId { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<ComplaintFeedback> ComplaintFeedbacks { get; set; } = new List<ComplaintFeedback>();

    public virtual Constituency Constituency { get; set; } = null!;

    public virtual ICollection<DailyComplaintStatus> DailyComplaintStatuses { get; set; } = new List<DailyComplaintStatus>();

    public virtual User Leader { get; set; } = null!;
}
