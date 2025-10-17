using AutoMapper;
using WelfareTracker.Core.Contracts.Repository;
using WelfareTracker.Core.Contracts.Service;
using WelfareTracker.Core.Dtos;
using WelfareTracker.Core.Exceptions;
using WelfareTracker.Core.Models;
using WelfareTracker.Core.Vms;

namespace WelfareTracker.Infrastructure.Service;
public class UserService(IUserRepository userRepository,
                            IConstituencyRepository constituencyRepository,
                            IRoleRepository roleRepository,
                            IAuthService authService,
                            IMapper mapper) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IConstituencyRepository _constituencyRepository = constituencyRepository;
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IAuthService _authService = authService;
    private readonly IMapper _mapper = mapper;
    public async Task<UserDto> AddUserAsync(RegisterUserVm registerUserVm)
    {
        var passwordSalt = _authService.GenerateSalt();
        var passwordHash = _authService.GeneratePasswordHash(registerUserVm.Password, passwordSalt);
        var constituency = await _constituencyRepository.GetConstituencyByNameAsync(registerUserVm.ConstituencyName);
        var role = await _roleRepository.GetRoleByNameAsync(registerUserVm.RoleName);


        var user = new User
        {
            FullName = registerUserVm.FullName,
            Email = registerUserVm.Email,
            Age = registerUserVm.Age,
            MobileNumber = registerUserVm.MobileNumber,
            Gender = registerUserVm.Gender,
            Address = registerUserVm.Address,
            ConstituencyId = constituency.ConstituencyId,
            PasswordSalt = passwordSalt,
            PasswordHash = passwordHash,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
            RoleId = role.RoleId,
            RoleName = role.RoleName,
            ConstituencyName = constituency.ConstituencyName
        };

        var addedUser = await _userRepository.AddUserAsync(user);

        var userDto = _mapper.Map<UserDto>(addedUser);

        return userDto;
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            return false;
        }
        
        await _userRepository.DeleteUserAsync(user);
        return true;
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        var userDto = _mapper.Map<UserDto>(user);
        return userDto;
    }

    public async Task<LoginUserDto> LoginUser(LoginUserVm loginUserVm)
    {
        var user = await _userRepository.GetUserByEmailAsync(loginUserVm.Email);
        if (user == null)
        {
            throw new WelfareTrackerException("Invalid email or password.");
        }

        if(!_authService.VerifyPassword(loginUserVm.Password, user.PasswordSalt, user.PasswordHash))
        {
            throw new WelfareTrackerException("Invalid email or password.");
        }

        var token = _authService.GenerateJwtToken(user);
        var refreshToken = _authService.GenerateRefreshToken();

        var loginUserDto = new LoginUserDto
        {
            Token = token,
            RefreshToken = refreshToken
        };

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        await _userRepository.UpdateUserAsync(user);

        return loginUserDto;

    }

    public async Task<UserDto?> UpdateUserAsync(UserVm userVm)
    {
        var user = _mapper.Map<User>(userVm);
        var existingUser = await _userRepository.GetUserByIdAsync(user.UserId);
        if (existingUser == null)
        {
            return null;
        }
        await _userRepository.UpdateUserAsync(user);
        var userDto = _mapper.Map<UserDto>(user);
        return userDto;
    }
}
