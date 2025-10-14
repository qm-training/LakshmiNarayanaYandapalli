namespace JwtAuthentication.Core.Contracts.Services;
public interface IUserService
{
    Task<UserVm?> GetUserByName(string userName);
    Task<string> GetPassword(string userName);
}
