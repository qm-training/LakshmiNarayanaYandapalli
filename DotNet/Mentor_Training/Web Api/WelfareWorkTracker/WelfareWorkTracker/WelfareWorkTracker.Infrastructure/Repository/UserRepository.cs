namespace WelfareWorkTracker.Infrastructure.Repository;
public class UserRepository(WelfareWorkTrackerContext context) : IUserRepository
{
    private readonly WelfareWorkTrackerContext _context = context;

    public Task<User?> GetAdminAsync()
    {
        var admin = _context.Users.Where(c => c.RoleId == (int)Roles.Admin).FirstOrDefaultAsync();
        return admin;
    }

    public async Task<User?> GetAdminRepByConstituencyIdAsync(int constituencyId)
    {
        var adminRep = await _context.Users.Where(c => c.ConstituencyId == constituencyId && c.RoleId == (int)Roles.AdminRepresentative).FirstOrDefaultAsync();
        return adminRep;
    }

    public async Task<List<User>?> GetAllLeadersAsync()
    {
        var leaders = await _context.Users
                                    .Where(l => l.RoleId == (int)Roles.Leader)
                                    .ToListAsync();

        return leaders;
    }

    public async Task<int> GetCitizenCountInConstituencyAsync(int constituencyId)
    {
        int constituencyCount = await _context.Users
                                    .Where(c => c.ConstituencyId == constituencyId && c.RoleId == 4)
                                    .CountAsync();

        return constituencyCount;
    }

    public async Task<User?> GetLeaderByConstituencyIdAsync(int constituencyId)
    {
        var leader = await _context.Users
                                    .Where(c => c.ConstituencyId == constituencyId && c.RoleId == (int)Roles.Leader)
                                    .FirstOrDefaultAsync();

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

    public async Task<List<User>?> GetUsersByConstituencyIdAsync(int constituencyId)
    {
        var citizens = await _context.Users
                                     .Where(c => c.ConstituencyId == constituencyId && c.RoleId == (int)Roles.Citizen)
                                     .ToListAsync();

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