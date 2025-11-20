namespace WelfareWorkTracker.Core.Models;
public class ComplaintImage
{
    public int ComplaintImageId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public int ComplaintId { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

}
