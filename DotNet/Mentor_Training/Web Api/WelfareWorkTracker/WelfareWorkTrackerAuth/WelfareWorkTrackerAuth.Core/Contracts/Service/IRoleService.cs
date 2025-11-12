namespace WelfareWorkTrackerAuth.Core.Contracts.Service;
public interface IRoleService
{
    Task<RoleDto> AddRoleAsync(RoleVm roleVm);
    Task<List<RoleDto>> GetRolesAsync();
}