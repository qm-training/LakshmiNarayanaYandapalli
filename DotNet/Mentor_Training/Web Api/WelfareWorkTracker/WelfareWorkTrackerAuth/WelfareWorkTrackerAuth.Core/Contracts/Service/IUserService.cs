namespace WelfareWorkTrackerAuth.Core.Contracts.Service;
public interface IUserService
{
    Task<bool> RegisterAsync(RegisterUserVm registerUserVm);
    Task<LoginUserDto> LoginAsync(LoginUserVm loginUserVm);
    Task<bool> ResetPasswordAsync(ResetPasswordVm resetPasswordVm);
    Task<LoginUserDto> RenewAccessTokenAsync(string email, string refreshToken);
}