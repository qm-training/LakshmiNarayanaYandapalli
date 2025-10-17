using System;
using System.Collections.Generic;

namespace WelfareTracker.Core.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public string Description { get; set; } = null!;

    public int UserId { get; set; }

    public int? ComplaintId { get; set; }

    public int? DailyComplaintId { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

    public bool IsAnonymous { get; set; }

    public virtual Complaint? Complaint { get; set; }

    public virtual DailyComplaint? DailyComplaint { get; set; }

    public virtual User User { get; set; } = null!;
}
