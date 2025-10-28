namespace WelfareWorkTracker.Core.Dtos
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        public string Description { get; set; } = null!;
        public int? UserId { get; set; } = 0;
        public int? ComplaintId { get; set; } = 0;
        public int? DailyComplaintId { get; set; } = 0;
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
