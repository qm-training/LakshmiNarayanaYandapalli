namespace WelfareWorkTracker.Core.Dtos;
public class CommentDto
{
    public int CommentId { get; set; }

    public string Description { get; set; } = null!;

    public int UserId { get; set; }

    public int? ComplaintId { get; set; }

    public int? DailyComplaintId { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

    public bool IsAnonymous { get; set; }
}