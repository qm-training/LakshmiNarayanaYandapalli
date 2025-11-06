using Microsoft.EntityFrameworkCore;
using System.Net;
using WelfareWorkTracker.Core.Contracts.Repository;
using WelfareWorkTracker.Core.Exceptions;
using WelfareWorkTracker.Core.Models;
using WelfareWorkTracker.Infrastructure.Data;

namespace WelfareWorkTracker.Infrastructure.Repository
{
    public class EmailTemplateRepository(WelfareWorkTrackerContext context) : IEmailTemplateRepository
    {
        private readonly WelfareWorkTrackerContext _context = context;

        public async Task<EmailTemplate> GetByIdAsync(int id)
        {
            var template = await _context.EmailTemplates
            .Where(t => t.IsActive)
            .FirstOrDefaultAsync(t => t.Id == id) 
            ?? throw new WelfareWorkTrackerException($"Email Template with Id {id} Not Found!", (int)HttpStatusCode.NotFound);
            return template;
        }

        public async Task<List<EmailTemplate>> GetAllAsync()
        {
            return await _context.EmailTemplates
            .Where(t => t.IsActive)
            .ToListAsync();
        }

        public async Task<EmailTemplate> GetByNameAsync(string templateName)
        {
            var template = await _context
                .EmailTemplates
                .Where(t => t.IsActive)
                .FirstOrDefaultAsync(t => t.Name == templateName) 
                ?? throw new WelfareWorkTrackerException($"Email Template with template name {templateName} Not Found!", (int)HttpStatusCode.NotFound);
            return template;
        }

        public async Task<bool> AddAsync(EmailTemplate template)
        {
            await _context.EmailTemplates.AddAsync(template);
            var rowsEffected = await _context.SaveChangesAsync();
            return rowsEffected > 0;
        }

        public async Task<bool> UpdateAsync(EmailTemplate template)
        {

            var placeholder = await _context.EmailTemplates.FindAsync(template.Id) 
                ?? throw new WelfareWorkTrackerException($"Email Templates with Id {template.Id} Not Found!", (int)HttpStatusCode.NotFound);

            _context.EmailTemplates.Update(template);
            var rowsEffected = await _context.SaveChangesAsync();
            return rowsEffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var template = await _context.EmailTemplates.FindAsync(id) 
                ?? throw new WelfareWorkTrackerException($"Email Templates with Id {id} Not Found!", (int)HttpStatusCode.NotFound);
            _context.EmailTemplates.Remove(template);
            var rowsEffected = await _context.SaveChangesAsync();
            return rowsEffected > 0;
        }
    }

}
