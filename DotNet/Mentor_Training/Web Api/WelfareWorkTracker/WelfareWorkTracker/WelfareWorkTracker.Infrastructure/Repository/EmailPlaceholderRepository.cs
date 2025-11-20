namespace WelfareWorkTracker.Infrastructure.Repository;
public class EmailPlaceholderRepository(WelfareWorkTrackerContext context) : IEmailPlaceholderRepository
{
    private readonly WelfareWorkTrackerContext _context = context;

    public async Task<EmailPlaceholder> GetByIdAsync(int id)
    {
        return await _context.EmailPlaceholders
            .FirstOrDefaultAsync(epd => epd.Id == id)
            ?? throw new WelfareWorkTrackerException($"Email Placeholder with Id {id} was not found!", (int)HttpStatusCode.NotFound);
    }

    public async Task<List<EmailPlaceholder>> GetAllAsync()
    {
        return await _context.EmailPlaceholders
            .ToListAsync();
    }

    public async Task<List<EmailPlaceholder>> GetByEmailOutboxIdAsync(int emailOutboxId)
    {
        return await _context.EmailPlaceholders
            .Where(epd => epd.EmailOutboxId == emailOutboxId)
            .ToListAsync();
    }

    public async Task<bool> AddAsync(EmailPlaceholder placeholderData)
    {
        await _context.EmailPlaceholders.AddAsync(placeholderData);
        var rowsEffected = await _context.SaveChangesAsync();
        return rowsEffected > 0;
    }


    public async Task<bool> UpdateAsync(EmailPlaceholder placeholderData)
    {
        var placeholder = await _context.EmailPlaceholders.FindAsync(placeholderData.Id);

        if (placeholder == null) {
            throw new WelfareWorkTrackerException($"Email Placeholder with Id {placeholderData.Id} Not Found!", (int)HttpStatusCode.NotFound);
        }

        _context.EmailPlaceholders.Update(placeholderData);
        var rowsEffected = await _context.SaveChangesAsync();
        return rowsEffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var placeholderData = await _context.EmailPlaceholders.FindAsync(id)
            ?? throw new WelfareWorkTrackerException($"Email Placeholder with Id {id} Not Found!", (int)HttpStatusCode.NotFound);
        _context.EmailPlaceholders.Remove(placeholderData);
        var rowsEffected = await _context.SaveChangesAsync();
        return rowsEffected > 0;
    }

    public async Task<bool> AddRangeAsync(List<EmailPlaceholder> placeholderData)
    {
        await _context.EmailPlaceholders.AddRangeAsync(placeholderData);
        var rowsEffected = await _context.SaveChangesAsync();
        return rowsEffected > 0;
    }
}