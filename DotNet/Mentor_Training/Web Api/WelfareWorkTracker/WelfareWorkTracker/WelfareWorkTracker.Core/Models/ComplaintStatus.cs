namespace WelfareWorkTracker.Core.Models;
public class ComplaintStatus
{
    public int ComplaintStatusId { get; set; }

    public DateTime? OpenedDate { get; set; }

    public DateTime? DeadlineDate { get; set; }

    public int AttemptNumber { get; set; }

    public int ComplaintId { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

    public int Status { get; set; }

    public string? RejectReason { get; set; }

    public DateTime? NewOpenedDate { get; set; }

}
