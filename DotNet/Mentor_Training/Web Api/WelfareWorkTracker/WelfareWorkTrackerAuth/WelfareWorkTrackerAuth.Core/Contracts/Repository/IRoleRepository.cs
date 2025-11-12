namespace WelfareWorkTrackerAuth.Core.Contracts.Repository;
public interface IRoleRepository
{
    Task<Role> AddRoleAsync(Role role);
    Task<List<Role>> GetRolesAsync();
}