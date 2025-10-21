namespace WelfareTracker.Core.Vms
{
    public class CommentVm
    {
        public string Description { get; set; } = null!;
        public int? ComplaintId { get; set; } = 0;
        public int? DailyComplaintId { get; set; } = 0;
        public bool IsAnonymous { get; set; }
    }
}
