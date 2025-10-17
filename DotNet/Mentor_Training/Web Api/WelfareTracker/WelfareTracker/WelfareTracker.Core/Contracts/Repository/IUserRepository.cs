using WelfareTracker.Core.Models;

namespace WelfareTracker.Core.Contracts.Repository;
public interface IUserRepository
{
    Task<User> AddUserAsync(User user);
    Task<User> UpdateUserAsync(User user);
    Task<User> DeleteUserAsync(User user);
    Task<User?> GetUserByIdAsync(int userId);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetLeaderByConstituencyName(string constituencyName);
}
