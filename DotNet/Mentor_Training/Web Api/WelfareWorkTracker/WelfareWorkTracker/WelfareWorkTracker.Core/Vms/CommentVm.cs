namespace WelfareWorkTracker.Core.Vms
{
    public class CommentVm
    {
        public string Description { get; set; } = null!;
        public int? ComplaintId { get; set; }
        public int? DailyComplaintId { get; set; }
        public bool IsAnonymous { get; set; }
    }
}
