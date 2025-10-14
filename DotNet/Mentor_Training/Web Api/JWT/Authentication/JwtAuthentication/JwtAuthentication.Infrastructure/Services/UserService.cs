namespace JwtAuthentication.Infrastructure.Services;
public class UserService(IUserRepository repository) : IUserService
{
    private readonly IUserRepository _repository = repository;

    public async Task<string> GetPassword(string userName)
    {
        var user = await _repository.GetUserByNameAsync(userName);
        if (user == null)
        {
            return null;
        }
        return user.Password;
    }

    public async Task<UserVm?> GetUserByName(string userName)
    {
        var user = await _repository.GetUserByNameAsync(userName);
        if (user == null)
        {
            return null;
        }

        return new UserVm
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role?.RoleName ?? string.Empty
        };

    }
}
