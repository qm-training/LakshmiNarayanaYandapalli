namespace WelfareWorkTracker.Core.Contracts.Service;
public interface IRoleService
{
    Task<List<RoleDto>> GetRolesAsync();
    Task<RoleDto> AddRoleAsync(RoleVm roleVm);
}