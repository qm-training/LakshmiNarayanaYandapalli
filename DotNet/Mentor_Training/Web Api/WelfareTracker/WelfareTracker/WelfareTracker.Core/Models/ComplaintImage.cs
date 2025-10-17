using System;
using System.Collections.Generic;

namespace WelfareTracker.Core.Models;

public partial class ComplaintImage
{
    public int ComplaintImageId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public int ComplaintId { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

    public virtual Complaint Complaint { get; set; } = null!;
}
