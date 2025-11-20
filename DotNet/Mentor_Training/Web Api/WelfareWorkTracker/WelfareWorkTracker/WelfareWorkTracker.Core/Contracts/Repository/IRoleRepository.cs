namespace WelfareWorkTracker.Core.Contracts.Repository;
public interface IRoleRepository
{
    Task<Role> AddRoleAsync(Role role);
    Task<List<Role>> GetAllRolesAsync();
    Task<int> GetRoleIdByRoleNameAsync(string roleName);
}