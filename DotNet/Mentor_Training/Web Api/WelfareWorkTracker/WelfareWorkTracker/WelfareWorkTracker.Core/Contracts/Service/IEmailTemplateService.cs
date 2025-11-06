using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WelfareWorkTracker.Core.Models;
using WelfareWorkTracker.Core.Vms;

namespace WelfareWorkTracker.Core.Contracts.Service
{
    public interface IEmailTemplateService
    {
        Task<EmailTemplate> GetTemplateByIdAsync(int id);
        Task<EmailTemplate> GetByNameAsync(string templateName);
        Task<IEnumerable<EmailTemplate>> GetAllTemplatesAsync();
        Task AddTemplateAsync(int userId, CreateEmailTemplateVm template);
        Task UpdateTemplateAsync(int templateId, int userId, UpdateEmailTemplateVm template);
        Task DeleteTemplateAsync(int templateId, int userId);
    }
}
