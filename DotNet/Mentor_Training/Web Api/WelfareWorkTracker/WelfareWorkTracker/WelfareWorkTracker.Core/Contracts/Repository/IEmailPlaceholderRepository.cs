using WelfareWorkTracker.Core.Models;

namespace WelfareWorkTracker.Core.Contracts.Repository
{
    public interface IEmailPlaceholderRepository
    {
        Task<EmailPlaceholder> GetByIdAsync(int id);
        Task<List<EmailPlaceholder>> GetAllAsync();
        Task<List<EmailPlaceholder>> GetByEmailHistoryIdAsync(int emailHistoryId);
        Task<bool> AddAsync(EmailPlaceholder placeholderData);
        Task<bool> AddRangeAsync(List<EmailPlaceholder> placeholderData);
        Task<bool> UpdateAsync(EmailPlaceholder placeholderData);
        Task<bool> DeleteAsync(int id);
    }
}
