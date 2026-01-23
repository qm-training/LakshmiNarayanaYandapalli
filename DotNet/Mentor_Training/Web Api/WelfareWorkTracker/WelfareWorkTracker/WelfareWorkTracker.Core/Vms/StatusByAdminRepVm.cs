namespace WelfareWorkTracker.Core.Vms;
public class StatusByAdminRepVm
{
    public int ComplaintId { get; set; }

    public int Status { get; set; }

    public string? RejectReason { get; set; }

    public int ReferenceNumber { get; set; } = 0;

    public string? ExpectedDeadline { get; set; }

    public string? MaxExtendableDeadline { get; set; }
}