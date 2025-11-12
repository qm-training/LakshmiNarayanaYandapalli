namespace WelfareWorkTracker.Core.Contracts.Repository;
public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(int userId);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetLeaderByConstituencyIdAsync(int constituencyId);
    Task<List<User>?> GetUsersByConstituencyIdAsync(int constituencyId);
    Task<User?> GetAdminRepByConstituencyIdAsync(int constituencyId);
    Task<List<User>?> GetAllLeadersAsync();
    Task<User> UpdateUserAsync(User user);
    Task<User> UpdateLeaderReputationAsync(int leaderId, double reputation);
    Task<int> GetCitizenCountInConstituencyAsync(int constituencyId);
    Task<User?> GetAdminAsync();

}