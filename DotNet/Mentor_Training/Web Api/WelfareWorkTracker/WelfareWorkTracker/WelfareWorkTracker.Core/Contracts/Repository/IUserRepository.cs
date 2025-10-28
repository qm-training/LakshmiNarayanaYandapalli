using WelfareWorkTracker.Core.Models;

namespace WelfareWorkTracker.Core.Contracts.Repository
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(int userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetLeaderByConstituencyNameAsync(string constituencyName);
        Task<List<User>?> GetUsersByConstituencyNameAsync(string constituencyName);
        Task<User?> GetAdminRepByConstituencyName(string constituencyName);
        Task<List<User>?> GetAllLeadersAsync();
        Task<User> UpdateUserAsync(User user);
        Task<User> UpdateLeaderReputationAsync(int leaderId, double reputation);

    }
}
