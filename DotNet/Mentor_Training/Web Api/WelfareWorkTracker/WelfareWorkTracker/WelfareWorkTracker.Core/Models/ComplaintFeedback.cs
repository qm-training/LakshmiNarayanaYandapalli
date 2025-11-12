namespace WelfareWorkTracker.Core.Models;
public class ComplaintFeedback
{
    public int CitizenFeedbackId { get; set; }

    public string? FeedbackMessage { get; set; }

    public bool IsSatisfied { get; set; }

    public int? ComplaintId { get; set; }

    public int? DailyComplaintId { get; set; }

    public int CitizenId { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

}
