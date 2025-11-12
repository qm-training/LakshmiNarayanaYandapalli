namespace WelfareWorkTracker.Core.Dtos;
public class ConstituencyDto
{
    public int ConstituencyId { get; set; }

    public string ConstituencyName { get; set; } = null!;

    public string DistrictName { get; set; } = null!;

    public string StateName { get; set; } = null!;

    public string CountryName { get; set; } = null!;

    public long Pincode { get; set; }
}