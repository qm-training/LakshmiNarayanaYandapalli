namespace WelfareWorkTracker.Core.Models;
public class EmailOutbox
{
    public int Id { get; set; }

    public int EmailTemplateId { get; set; }

    public string ToEmail { get; set; } = null!;

    public string? FromEmail { get; set; }

    public DateTime SentAt { get; set; }

}
