namespace WelfareWorkTrackerAuth.Infrastructure.Repository;
public class RoleRepository(WelfareWorkTrackerContext context) : IRoleRepository
{
    private readonly WelfareWorkTrackerContext _context = context;
    public async Task<Role> AddRoleAsync(Role role)
    {
        await _context.Roles.AddAsync(role);
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task<List<Role>> GetRolesAsync()
    {
        var roles = await _context.Roles.ToListAsync();
        return roles;
    }
}
