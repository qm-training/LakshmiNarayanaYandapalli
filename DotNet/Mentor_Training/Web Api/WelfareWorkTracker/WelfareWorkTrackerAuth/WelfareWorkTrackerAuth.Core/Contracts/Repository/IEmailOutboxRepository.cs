namespace WelfareWorkTrackerAuth.Core.Contracts.Repositories;

public interface IEmailOutboxRepository
{
    Task<EmailOutbox> GetByIdAsync(int id);
    Task<List<EmailOutbox>> GetAllAsync();
    Task<List<EmailOutbox>> GetByTemplateIdAsync(int templateId);
    Task<bool> AddAsync(EmailOutbox emailOutbox);
    Task<bool> UpdateAsync(EmailOutbox emailOutbox);
    Task<bool> DeleteAsync(int id);
}
