namespace WelfareWorkTrackerAuth.Infrastructure.Service;
public class UserService(IUserRepository userRepository,
                            IConstituencyRepository constituencyRepository,
                            IAuthService authService) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IConstituencyRepository _constituencyRepository = constituencyRepository;
    private readonly IAuthService _authService = authService;
    public async Task<LoginUserDto> LoginAsync(LoginUserVm loginUserVm)
    {
        var user = await _userRepository.CheckIfUserEmailExistsAsync(loginUserVm.Email)
                                ?? throw new WelfareWorkTrackerException("Invalid email or password.");

        if (!_authService.VerifyPassword(loginUserVm.Password, user.PasswordSalt, user.PasswordHash))
        {
            throw new WelfareWorkTrackerException("Invalid email or password.");
        }

        var accessToken = _authService.GenerateJwtToken(user);
        var refreshToken = _authService.GenerateRefreshToken();

        var loginUserDto = new LoginUserDto
        {
            Accesstoken = accessToken,
            RefreshToken = refreshToken
        };

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        await _userRepository.UpdateUserAsync(user);

        return loginUserDto;
    }

    public async Task<bool> RegisterAsync(RegisterUserVm registerUserVm)
    {
        var passwordSalt = _authService.GenerateSalt();
        var passwordHash = _authService.GeneratePasswordHash(registerUserVm.Password, passwordSalt);
        var constituencyId = await _constituencyRepository.GetConstituencyIdByNameAsync(registerUserVm.ConstituencyName);
        int roleId = (int)Enum.Parse<Roles>(registerUserVm.RoleName, true);


        var user = new User
        {
            FullName = registerUserVm.FullName,
            Email = registerUserVm.Email,
            Age = registerUserVm.Age,
            MobileNumber = registerUserVm.MobileNumber,
            Gender = registerUserVm.Gender,
            Address = registerUserVm.Address,
            ConstituencyId = constituencyId,
            PasswordSalt = passwordSalt,
            PasswordHash = passwordHash,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
            RoleId = roleId,
            RoleName = registerUserVm.RoleName,
            ConstituencyName = registerUserVm?.ConstituencyName
        };

        await _userRepository.RegisterUserAsync(user);
        return true;
        
    }

    public async Task<LoginUserDto> RenewAccessTokenAsync(string email, string refreshToken)
    {
        var user = await _userRepository.CheckIfUserEmailExistsAsync(email)
                    ?? throw new WelfareWorkTrackerException("Invalid email.");

        if (!user.RefreshToken!.Equals(refreshToken) || user.RefreshTokenExpiry < DateTime.UtcNow)
        {
            throw new WelfareWorkTrackerException("Refresh not valid.");
        }

        var accessToken = _authService.GenerateJwtToken(user);
        var newRefreshToken = _authService.GenerateRefreshToken();

        var loginUserDto = new LoginUserDto
        {
            Accesstoken = accessToken,
            RefreshToken = newRefreshToken
        };

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        await _userRepository.UpdateUserAsync(user);

        return loginUserDto;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordVm resetPasswordVm)
    {
        var user = await _userRepository.CheckIfUserEmailExistsAsync(resetPasswordVm.Email)
                            ?? throw new WelfareWorkTrackerException("Invalid email.");

        if (!_authService.VerifyPassword(resetPasswordVm.OldPassword, user.PasswordSalt, user.PasswordHash))
        {
            throw new WelfareWorkTrackerException("Incorrect password.");
        }

        var passwordSalt = _authService.GenerateSalt();
        var passwordHash = _authService.GeneratePasswordHash(resetPasswordVm.NewPassword, passwordSalt);

        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        await _userRepository.UpdateUserAsync(user);
        return true;
    }
}
