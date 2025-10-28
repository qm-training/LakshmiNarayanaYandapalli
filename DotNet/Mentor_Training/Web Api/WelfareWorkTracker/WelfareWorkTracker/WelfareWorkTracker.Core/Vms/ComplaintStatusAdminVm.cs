namespace WelfareWorkTracker.Core.Vms
{
    public class ComplaintStatusAdminVm
    {
        public int ComplaintId { get; set; }
        public int Status { get; set; }
        public string? RejectReason { get; set; }
    }
}
