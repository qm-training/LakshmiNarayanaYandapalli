namespace JwtAuthentication.Core.Contracts.Services;
public interface IUserService
{
    Task<UserVm?> GetUserByNameAsync(string userName);
    Task<string> AddUserAsync(UserDto userDto);
    Task<bool> PasswordCheckAsync(string userName, string password);
}
