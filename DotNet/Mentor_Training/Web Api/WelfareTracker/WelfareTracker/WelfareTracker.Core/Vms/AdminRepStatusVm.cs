namespace WelfareTracker.Core.Vms
{
    public class AdminRepStatusVm
    {
        public int ComplaintId { get; set; }

        public int Status { get; set; }

        public string? RejectReason { get; set; }

        public int ReferenceNumber { get; set; } = 0;

        public int ExpectedDeadline { get; set; }

        public string? MaxExtendableDeadline { get; set; }
    }
}
