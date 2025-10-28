using Microsoft.EntityFrameworkCore;
using System.Net;
using WelfareWorkTracker.Core.Contracts.Repository;
using WelfareWorkTracker.Core.Models;
using WelfareWorkTracker.Core.Vms;
using WelfareWorkTracker.Infrastructure.Data;

namespace WelfareWorkTracker.Infrastructure.Repository
{
    public class UserRepository(WelfareWorkTrackerContext context) : IUserRepository
    {
        private readonly WelfareWorkTrackerContext _context = context;
        public async Task<User?> GetAdminRepByConstituencyName(string constituencyName)
        {
            var adminRep = await _context.Users.Where(c => c.ConstituencyName == constituencyName && c.RoleName == "AdminRepresentative").FirstOrDefaultAsync();
            return adminRep;
        }

        public async Task<List<User>?> GetAllLeadersAsync()
        {
            var leaders = await _context.Users.Where(l => l.RoleName == "Leader").ToListAsync();
            return leaders;
        }

        public async Task<User?> GetLeaderByConstituencyNameAsync(string constituencyName)
        {
            var leader = await _context.Users.Where(c => c.ConstituencyName == constituencyName && c.RoleName == "Leader").FirstOrDefaultAsync();
            return leader;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(c => c.Email == email);
            return user;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user;
        }

        public async Task<List<User>?> GetUsersByConstituencyNameAsync(string constituencyName)
        {
            var citizens = await _context.Users.Where(c => c.ConstituencyName == constituencyName && c.RoleName == "Citizen").ToListAsync();
            return citizens;
        }

        public async Task<User> UpdateLeaderReputationAsync(int leaderId, double reputation)
        {
            var leader = await _context.Users.FirstOrDefaultAsync(c => c.UserId == leaderId);
            leader!.Reputation = reputation;
            _context.Users.Update(leader);
            await _context.SaveChangesAsync();

            return leader;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
