namespace JwtAuthentication.Core.Contracts.Repository;
public interface IUserRepository
{
    Task<User?> GetUserByNameAsync(string username);
    Task<string> AddUser(User user);
}
