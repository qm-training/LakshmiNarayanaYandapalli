namespace WelfareWorkTracker.Core.Vms
{
    public class UserVm
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int? Age { get; set; }
        public long MobileNumber { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string RoleName { get; set; } = null!;
        public string ConstituencyName { get; set; } = null!;
    }
}
