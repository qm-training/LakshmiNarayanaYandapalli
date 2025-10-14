namespace JwtAuthentication.Infrastructure.Services;
public class UserService(IUserRepository repository, IRoleRepository roleRepository) : IUserService
{
    private readonly IUserRepository _repository = repository;
    private readonly IRoleRepository _roleRepository = roleRepository;

    public async Task<UserVm?> GetUserByNameAsync(string userName)
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

    public async Task<string> AddUserAsync(UserDto userDto)
    {
        var role = await _roleRepository.GetRoleByNameAsync(userDto.RoleName);

        if (role == null)
        {
            return "Role not found";
        }

        var (hash, salt) = PasswordService.HashPassword(userDto.Password);

        var user = new User
        {
            Username = userDto.Username,
            PasswordHash = hash,
            Salt = salt,
            Email = userDto.Email,
            RoleId = role.RoleId
        };

        return await _repository.AddUser(user);
    }

    public async Task<bool> PasswordCheckAsync(string userName, string password)
    {
        var user = await _repository.GetUserByNameAsync(userName);
        if (user == null || user.PasswordHash == null || user.Salt == null)
        {
            return false;
        }
        return PasswordService.VerifyPassword(password, user.PasswordHash!, user.Salt!);
    }
}
