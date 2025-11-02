namespace WelfareWorkTrackerAuth.Core.Vms;
public class ResetPasswordVm
{
    public string Email { get; set; } = null!;
    public string OldPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
