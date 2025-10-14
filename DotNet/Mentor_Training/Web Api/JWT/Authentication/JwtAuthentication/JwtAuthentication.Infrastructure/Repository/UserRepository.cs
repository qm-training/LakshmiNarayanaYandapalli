namespace JwtAuthentication.Infrastructure.Repository;
public class UserRepository(JwtContext context) : IUserRepository
{
    private readonly JwtContext _context = context;
    public async Task<User?> GetUserByNameAsync(string username)
    {
        return await _context.Users
        .Include(u => u.Role)
        .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<string> AddUser(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return "user added successfully";

    }
}
