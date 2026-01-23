namespace WelfareWorkTracker.Infrastructure.Service;
public class EmailTemplateService(IEmailTemplateRepository repository, IMapper mapper) : IEmailTemplateService
{
    private readonly IEmailTemplateRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public async Task<EmailTemplate> GetTemplateByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<List<EmailTemplate>> GetAllTemplatesAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task AddTemplateAsync(int userId, CreateEmailTemplateVm template)
    {
        var emailTemplate = _mapper.Map<EmailTemplate>(template);
        emailTemplate.DateCreated = DateTime.UtcNow;
        emailTemplate.CreatedBy = userId;
        await _repository.AddAsync(emailTemplate);
    }

    public async Task UpdateTemplateAsync(int templateId, int userId, UpdateEmailTemplateVm template)
    {
        var emailTemplateFound = await _repository.GetByIdAsync(templateId)
                ?? throw new WelfareWorkTrackerException($"Email template with Id {templateId} Not Found.");

        if (emailTemplateFound.CreatedBy != userId) 
            throw new WelfareWorkTrackerException($"You don't have access to Update template with Id {templateId}");

        emailTemplateFound.DateUpdated = DateTime.UtcNow;
        emailTemplateFound.Subject = template.Subject;
        emailTemplateFound.Body = template.Body;

        await _repository.UpdateAsync(emailTemplateFound);
    }

    public async Task DeleteTemplateAsync(int templateId, int userId)
    {
        var emailTemplateFound = await _repository.GetByIdAsync(templateId)
            ?? throw new WelfareWorkTrackerException($"Email template with Id {templateId} Not Found.");

        if (emailTemplateFound.CreatedBy != userId) 
            throw new WelfareWorkTrackerException($"You don't have access to Delete template with Id {templateId}");

        emailTemplateFound.IsActive = false;
        emailTemplateFound.DateUpdated = DateTime.UtcNow;

        await _repository.UpdateAsync(emailTemplateFound);
    }

    public async Task<EmailTemplate> GetByNameAsync(string templateName)
    {
        return await _repository.GetByNameAsync(templateName);
    }
}
