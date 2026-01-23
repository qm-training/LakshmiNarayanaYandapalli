namespace WelfareWorkTracker.Core.Dtos;
public class ComplaintDto
{
    public int ComplaintId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string ConstituencyName { get; set; } = null!;

    public int Attempts { get; set; }

    public bool IsClosed { get; set; }

    public int CitizenId { get; set; }

    public int LeaderId { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

    public int ConstituencyId { get; set; }

    public string Status { get; set; } = null!;

    public List<string> Images { get; set; } = [];
}
