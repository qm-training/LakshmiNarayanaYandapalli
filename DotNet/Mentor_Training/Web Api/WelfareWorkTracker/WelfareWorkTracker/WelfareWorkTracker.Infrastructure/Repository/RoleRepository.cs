using Microsoft.EntityFrameworkCore;
using WelfareWorkTracker.Core.Contracts.Repository;
using WelfareWorkTracker.Core.Models;
using WelfareWorkTracker.Infrastructure.Data;

namespace WelfareWorkTracker.Infrastructure.Repository
{
    public class RoleRepository(WelfareWorkTrackerContext context) : IRoleRepository
    {
        private readonly WelfareWorkTrackerContext _context = context;
        public async Task<Role> AddRoleAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            var roles = await _context.Roles.ToListAsync();
            return roles;
        }

        public async Task<int> GetRoleIdByRoleNameAsync(string roleName)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName);
            return role == null ? 0 : role.RoleId;
        }
    }
}
