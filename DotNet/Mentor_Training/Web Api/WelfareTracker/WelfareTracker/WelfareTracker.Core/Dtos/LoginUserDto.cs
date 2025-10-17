namespace WelfareTracker.Core.Dtos
{
    public class LoginUserDto
    {
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;

    }
}
