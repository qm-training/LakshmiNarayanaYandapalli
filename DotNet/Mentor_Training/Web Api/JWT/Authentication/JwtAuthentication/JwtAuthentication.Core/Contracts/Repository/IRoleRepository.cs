namespace JwtAuthentication.Core.Contracts.Repository;
public interface IRoleRepository
{
    Task<Role> GetRoleByNameAsync(string roleName);
}
