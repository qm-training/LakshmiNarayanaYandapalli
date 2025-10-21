namespace WelfareTracker.Core.Vms
{
    public class AdminStatusVm
    {
        public int ComplaintId { get; set; }
        public int Status { get; set; }
        public string? RejectReason { get; set; }
    }
}
