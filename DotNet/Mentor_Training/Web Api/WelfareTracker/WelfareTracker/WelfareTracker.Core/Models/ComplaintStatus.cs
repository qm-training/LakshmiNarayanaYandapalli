using System;
using System.Collections.Generic;

namespace WelfareTracker.Core.Models;

public partial class ComplaintStatus
{
    public int ComplaintStatusId { get; set; }

    public int ComplaintId { get; set; }

    public int Status { get; set; }

    public int AttemptNumber { get; set; }

    public DateTime? DeadlineDate { get; set; }

    public DateTime? EscalatedDate { get; set; }

    public string? RejectReason { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

    public virtual Complaint Complaint { get; set; } = null!;
}
