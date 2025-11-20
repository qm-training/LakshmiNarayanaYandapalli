namespace WelfareWorkTrackerAuth.Core.Vms;
public class RenewAccessTokenVm
{
    public string Email { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}