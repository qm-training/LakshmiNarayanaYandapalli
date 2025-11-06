using WelfareWorkTracker.Core.Models;

namespace WelfareWorkTracker.Core.Contracts.Repository
{
    public interface IEmailTemplateRepository
    {
        Task<EmailTemplate> GetByIdAsync(int id);
        Task<EmailTemplate> GetByNameAsync(string templateName);
        Task<List<EmailTemplate>> GetAllAsync();
        Task<bool> AddAsync(EmailTemplate template);
        Task<bool> UpdateAsync(EmailTemplate template);
        Task<bool> DeleteAsync(int id);
    }
}
