namespace WelfareWorkTracker.Core.Vms
{
    public class ComplaintVm
    {
        public string Description { get; set; } = null!;
        public List<String> ComplaintImageUrls { get; set; } = [];
    }
}
