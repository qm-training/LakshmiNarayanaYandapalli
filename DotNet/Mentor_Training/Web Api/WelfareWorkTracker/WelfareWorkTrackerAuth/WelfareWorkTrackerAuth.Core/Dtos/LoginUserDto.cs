namespace WelfareWorkTrackerAuth.Core.Dtos;
public class LoginUserDto
{
    public string Accesstoken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}
