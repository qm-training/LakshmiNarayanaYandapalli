namespace JwtAuthentication.Infrastructure.Repository;
public class RoleRepository(JwtContext context) : IRoleRepository
{
    private readonly JwtContext _context = context;
    public async Task<Role> GetRoleByNameAsync(string roleName)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName);
    }
}
