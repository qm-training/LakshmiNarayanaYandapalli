namespace WelfareWorkTracker.Core.Contracts.Service;
public interface IEmailTemplateService
{
    Task<EmailTemplate> GetTemplateByIdAsync(int id);
    Task<EmailTemplate> GetByNameAsync(string templateName);
    Task<List<EmailTemplate>> GetAllTemplatesAsync();
    Task AddTemplateAsync(int userId, CreateEmailTemplateVm template);
    Task UpdateTemplateAsync(int templateId, int userId, UpdateEmailTemplateVm template);
    Task DeleteTemplateAsync(int templateId, int userId);
}
