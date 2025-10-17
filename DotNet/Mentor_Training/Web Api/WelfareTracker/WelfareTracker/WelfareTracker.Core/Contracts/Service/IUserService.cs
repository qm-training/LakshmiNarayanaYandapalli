using WelfareTracker.Core.Dtos;
using WelfareTracker.Core.Vms;

namespace WelfareTracker.Core.Contracts.Service;
public interface IUserService
{
    Task<UserDto> AddUserAsync(RegisterUserVm registerUserVm);
    Task<UserDto?> UpdateUserAsync(UserVm userVm);
    Task<bool> DeleteUserAsync(int userId);
    Task<UserDto?> GetUserByIdAsync(int userId);
    Task<LoginUserDto> LoginUser(LoginUserVm loginUserVm);
}
