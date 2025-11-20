namespace WelfareWorkTracker.Infrastructure.Service;
public class UserService(IUserRepository userRepository,
                            IRoleRepository roleRepository,
                            IConstituencyRepository constituencyRepository,
                            IMapper mapper) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IConstituencyRepository _constituencyRepository = constituencyRepository;
    private readonly IMapper _mapper = mapper;
    public async Task<UserDto> GetUserInfoByUserIdAsync(int userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId) 
            ?? throw new WelfareWorkTrackerException("User not found.", (int)HttpStatusCode.NotFound);

        var userDto = _mapper.Map<UserDto>(user);
        return userDto;
    }

    public async Task<UserDto> UpdateUserInfoAsyncAsync(int userId, UserVm userVm)
    {
        var user = await _userRepository.GetUserByIdAsync(userId) 
            ?? throw new WelfareWorkTrackerException("User not found.", (int)HttpStatusCode.NotFound);

        var roleId = await _roleRepository.GetRoleIdByRoleNameAsync(userVm.RoleName);
        if (roleId == 0)
            throw new WelfareWorkTrackerException($"No Role found with name {userVm.RoleName}", (int)HttpStatusCode.NotFound);

        var constituencyId = await _constituencyRepository.GetConstituencyIdByNameAsync(userVm.ConstituencyName);
        if (constituencyId == 0)
            throw new WelfareWorkTrackerException($"No constituency found with name {userVm.ConstituencyName}", (int)HttpStatusCode.NotFound);

        user.FullName = userVm.FullName;
        user.Email = userVm.Email;
        user.Age = userVm.Age;
        user.MobileNumber = userVm.MobileNumber;
        user.Gender = userVm.Gender;
        user.Address = userVm.Address;
        user.RoleId = await _roleRepository.GetRoleIdByRoleNameAsync(userVm.RoleName);
        user.ConstituencyId = await _constituencyRepository.GetConstituencyIdByNameAsync(userVm.ConstituencyName);
        user.RoleId = roleId;
        user.ConstituencyId = constituencyId;
        user.DateUpdated = DateTime.UtcNow;

        var updatedUser = await _userRepository.UpdateUserAsync(user);
        var userDto = _mapper.Map<UserDto>(updatedUser);
        return userDto;
    }
}