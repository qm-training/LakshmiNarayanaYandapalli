namespace WelfareTracker.Core.Models;

public partial class DailyComplaintStatus
{
    public int DailyComplaintStatusId { get; set; }

    public int DailyComplaintId { get; set; }

    public int Status { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

    public virtual DailyComplaint DailyComplaint { get; set; } = null!;
}
