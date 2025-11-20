namespace WelfareWorkTracker.Core.Vms;
public class FeedbackVm
{
    public string? FeedbackMessage { get; set; }

    public bool IsSatisfied { get; set; }

    public int? ComplaintId { get; set; }

    public int? DailyComplaintId { get; set; }
}
