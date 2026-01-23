namespace WelfareWorkTracker.Core.Vms;
public class ComplaintVm
{
    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string ConstituencyName { get; set; } = null!;

    public List<string> Images { get; set; } = [];
}
