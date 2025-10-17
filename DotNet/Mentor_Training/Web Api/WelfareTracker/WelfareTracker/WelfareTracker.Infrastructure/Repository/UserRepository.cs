using Microsoft.EntityFrameworkCore;
using WelfareTracker.Core.Contracts.Repository;
using WelfareTracker.Core.Models;
using WelfareTracker.Infrastructure.Data;

namespace WelfareTracker.Infrastructure.Repository;
public class UserRepository(WelfareTrackerContext context) : IUserRepository
{
    private readonly WelfareTrackerContext _context = context;

    public async Task<User> AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> DeleteUserAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user;
    }

    public async Task<User> UpdateUserAsync(User user)
    {   
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
        return user;
    }

    public async Task<User?> GetLeaderByConstituencyName(string constituencyName)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.ConstituencyName == constituencyName && u.RoleName == "Leader");
        return user;
    }
}
