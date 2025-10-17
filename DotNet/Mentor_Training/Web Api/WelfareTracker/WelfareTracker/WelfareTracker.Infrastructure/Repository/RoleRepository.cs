using Microsoft.EntityFrameworkCore;
using WelfareTracker.Core.Contracts.Repository;
using WelfareTracker.Core.Models;
using WelfareTracker.Infrastructure.Data;

namespace WelfareTracker.Infrastructure.Repository
{
    public class RoleRepository(WelfareTrackerContext context) : IRoleRepository
    {
        private readonly WelfareTrackerContext _context = context;
        public async Task<Role> GetRoleByNameAsync(string roleName)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == roleName);

            return role!;
        }
    }
}
