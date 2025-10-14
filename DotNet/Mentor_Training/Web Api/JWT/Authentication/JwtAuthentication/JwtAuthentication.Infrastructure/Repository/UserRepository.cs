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
}
