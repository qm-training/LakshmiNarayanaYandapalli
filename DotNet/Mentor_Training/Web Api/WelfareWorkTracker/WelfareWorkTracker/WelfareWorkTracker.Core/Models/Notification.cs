namespace WelfareWorkTracker.Core.Models;

public class Notification
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string NotificationType { get; set; } = null!;

    public bool IsRead { get; set; }

    public int ToUserId { get; set; }

    public int? FromUserId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }
}
