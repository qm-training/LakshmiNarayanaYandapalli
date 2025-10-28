﻿using WelfareWorkTracker.Core.Dtos;
using WelfareWorkTracker.Core.Vms;

namespace WelfareWorkTracker.Core.Contracts.Service
{
    public interface IUserService
    {
        Task<UserDto> GetUserInfoByUserIdAsync(int userId);
        Task<UserDto> UpdateUserInfoAsyncAsync(int userId, UserVm userVm);
    }
}
