namespace WelfareWorkTracker.Core.Models;
public class DailyComplaintStatus
{
    public int DailyComplaintStatusId { get; set; }

    public int DailyComplaintId { get; set; }

    public int Status { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }
}
