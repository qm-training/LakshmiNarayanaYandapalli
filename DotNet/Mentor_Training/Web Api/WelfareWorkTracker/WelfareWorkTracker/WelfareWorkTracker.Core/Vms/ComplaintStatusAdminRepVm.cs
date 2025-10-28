namespace WelfareWorkTracker.Core.Vms
{
    public class ComplaintStatusAdminRepVm
    {
        public int ComplaintId { get; set; }
        public int Status { get; set; }
        public int ReferenceNumber { get; set; } = 0;
        public int ExpectedDeadline { get; set; }
    }
}
