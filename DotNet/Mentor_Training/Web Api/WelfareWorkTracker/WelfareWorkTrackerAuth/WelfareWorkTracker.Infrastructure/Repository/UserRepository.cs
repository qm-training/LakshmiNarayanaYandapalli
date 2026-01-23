namespace WelfareWorkTrackerAuth.Infrastructure.Repository;
public class UserRepository(WelfareWorkTrackerContext context) : IUserRepository
{
    private readonly WelfareWorkTrackerContext _context = context;
    public async Task<User?> CheckIfUserEmailExistsAsync(string email)
    {
        var user = await _context.Users.Where(u => u.Email.Equals(email)).FirstOrDefaultAsync();
        return user;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user;
    }

    public async Task<bool> RegisterUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdatePasswordAsync(int userId, string salt, string password)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return false;
        }

        user.PasswordSalt = salt;
        user.PasswordHash = password;

        var rowsEffected = await _context.SaveChangesAsync();
        return rowsEffected > 0;
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }
}