namespace WelfareWorkTrackerAuth.Core.Contracts.Repository;
public interface IUserRepository
{
    Task<User?> CheckIfUserEmailExistsAsync(string email);
    Task<bool> RegisterUserAsync(User user);
    Task<bool> UpdatePasswordAsync(int userId, string salt, string password);
    Task<User?> GetUserByIdAsync(int userId);
    Task<bool> UpdateUserAsync(User user);

}
