using WelfareWorkTracker.Core.Models;

namespace WelfareWorkTracker.Core.Contracts.Repository
{
    public interface IEmailOutboxRepository
    {
        Task<EmailOutbox> GetByIdAsync(int id);
        Task<List<EmailOutbox>> GetAllAsync();
        Task<List<EmailOutbox>> GetByTemplateIdAsync(int templateId);
        Task<bool> AddAsync(EmailOutbox emailHistory);
        Task<bool> UpdateAsync(EmailOutbox emailHistory);
        Task<bool> DeleteAsync(int id);
    }
}
