namespace WelfareTracker.Core.Vms
{
    public class ComplaintVm
    {
        public string Description { get; set; } = null!;
        public List<string> ComplaintImageUrls { get; set; } = [];

    }
}
