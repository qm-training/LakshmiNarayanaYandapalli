namespace WelfareWorkTracker.Core.Models;
public class Complaint
{
    public int ComplaintId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string CountryName { get; set; } = null!;

    public string StateName { get; set; } = null!;

    public string DistrictName { get; set; } = null!;

    public string ConstituencyName { get; set; } = null!;

    public string VillageName { get; set; } = null!;

    public int Pincode { get; set; }

    public string Address { get; set; } = null!;

    public int Attempts { get; set; }

    public bool IsClosed { get; set; }

    public int CitizenId { get; set; }

    public int LeaderId { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

    public int ReferenceNumber { get; set; }

}
