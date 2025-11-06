using Microsoft.EntityFrameworkCore;
using System.Net;
using WelfareWorkTracker.Core.Contracts.Repository;
using WelfareWorkTracker.Core.Exceptions;
using WelfareWorkTracker.Core.Models;
using WelfareWorkTracker.Infrastructure.Data;

namespace WelfareWorkTracker.Infrastructure.Repository
{
    public class EmailOutboxRepository(WelfareWorkTrackerContext context) : IEmailOutboxRepository
    {
        private readonly WelfareWorkTrackerContext _context = context;

        public async Task<EmailOutbox> GetByIdAsync(int id)
        {
            var outbox = await _context.EmailOutboxes
                .FirstOrDefaultAsync(eh => eh.Id == id) 
                ?? throw new WelfareWorkTrackerException($"Email Outbox with Id {id} was Not Found!", (int)HttpStatusCode.NotFound);
            
            return outbox;
        }

        public async Task<List<EmailOutbox>> GetAllAsync()
        {
            return await _context.EmailOutboxes
                .ToListAsync();
        }

        public async Task<List<EmailOutbox>> GetByTemplateIdAsync(int templateId)
        {
            return await _context.EmailOutboxes
                .Where(eh => eh.EmailTemplateId == templateId)
                .ToListAsync();
        }

        public async Task<bool> AddAsync(EmailOutbox emailHistory)
        {
            await _context.EmailOutboxes.AddAsync(emailHistory);
            var rowsEffected = await _context.SaveChangesAsync();
            return rowsEffected > 0;
        }

        public async Task<bool> UpdateAsync(EmailOutbox emailHistory)
        {
            var emailOutbox = await _context.EmailOutboxes.FindAsync(emailHistory.Id) 
                                    ?? throw new WelfareWorkTrackerException($"Email Outbox with Id {emailHistory.Id} Not Found!", (int)HttpStatusCode.NotFound);
            _context.EmailOutboxes.Update(emailHistory);
            var rowsEffected = await _context.SaveChangesAsync();
            return rowsEffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var emailHistory = await _context.EmailOutboxes.FindAsync(id) 
                        ?? throw new WelfareWorkTrackerException($"Email Outbox with Id {id} not Found!", (int)HttpStatusCode.NotFound);

            _context.EmailOutboxes.Remove(emailHistory);
            var rowsEffected = await _context.SaveChangesAsync();
            return rowsEffected > 0;
        }
    }
}
