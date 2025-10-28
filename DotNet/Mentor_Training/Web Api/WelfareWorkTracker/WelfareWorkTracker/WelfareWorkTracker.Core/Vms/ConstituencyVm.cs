namespace WelfareWorkTracker.Core.Vms
{
    public class ConstituencyVm
    {
        public string ConstituencyName { get; set; } = null!;

        public string DistrictName { get; set; } = null!;

        public string StateName { get; set; } = null!;

        public string CountryName { get; set; } = null!;

        public long Pincode { get; set; }
    }
}
