using System;
using System.Collections.Generic;

namespace WelfareTracker.Core.Models;

public partial class ComplaintFeedback
{
    public int CitizenFeedbackId { get; set; }

    public string? FeedbackMessage { get; set; }

    public bool IsSatisfied { get; set; }

    public int? ComplaintId { get; set; }

    public int? DailyComplaintId { get; set; }

    public int CitizenId { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

    public virtual User Citizen { get; set; } = null!;

    public virtual Complaint? Complaint { get; set; }

    public virtual DailyComplaint? DailyComplaint { get; set; }
}
