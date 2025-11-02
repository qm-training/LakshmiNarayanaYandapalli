namespace WelfareWorkTrackerAuth.Core.Vms;
public class RegisterUserVm
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int Age { get; set; }
    public long MobileNumber { get; set; }
    public string Gender { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string RoleName { get; set; } = null!;
    public string ConstituencyName { get; set; } = null!;
    public string Password { get; set; } = null!;
}
