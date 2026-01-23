namespace WelfareWorkTracker.Library.HandlerContext;
public class WelfareWorkTrackerEvent
{
    public string UserEmail { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;
}
