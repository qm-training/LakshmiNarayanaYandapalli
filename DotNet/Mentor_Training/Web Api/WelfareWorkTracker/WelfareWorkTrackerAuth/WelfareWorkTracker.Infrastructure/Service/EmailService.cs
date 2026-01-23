namespace WelfareWorkTrackerAuth.Infrastructure.Service;
public class EmailService(IEmailProcessorService emailProcessor,
                            IEmailTemplateRepository emailTemplateRepository,
                            IEmailPlaceholderRepository emailPlaceholderRepository,
                            IEmailOutboxRepository emailHistoryRepository,
                            IEventPublisher eventPublisher) : IEmailService
{
    private readonly IEmailProcessorService _emailProcessor = emailProcessor;
    private readonly IEmailTemplateRepository _emailTemplateRepository = emailTemplateRepository;
    private readonly IEmailPlaceholderRepository _emailPlaceholderRepository = emailPlaceholderRepository;
    private readonly IEmailOutboxRepository _emailHistoryRepository = emailHistoryRepository;
    private readonly IEventPublisher _eventPublisher = eventPublisher;

    public async Task<bool> SendEmailAsync(EmailVm emailVm)
    {
        // 1. Retrieve the template by name
        var template = await _emailTemplateRepository.GetByIdAsync(emailVm.TemplateId)
                       ?? throw new WelfareWorkTrackerException($"Email Template with name '{emailVm.TemplateId}' not found.", (int)HttpStatusCode.NotFound);

        // 2. Insert initial EmailHistory record
        var emailOutbox = new EmailOutbox
        {
            EmailTemplateId = template.Id,
            ToEmail = emailVm.ToUserEmail,
            SentAt = DateTime.UtcNow,
            FromEmail = emailVm.FromUserEmail,
        };

        await _emailHistoryRepository.AddAsync(emailOutbox);

        // Use a List instead of IEnumerable
        var emailPlaceholders = new List<EmailPlaceholder>();

        // 3. Insert placeholder data
        foreach (var (key, value) in emailVm.Payload)
        {
            var placeholder = new EmailPlaceholder
            {
                EmailOutboxId = emailOutbox.Id,
                PlaceHolderKey = key,
                PlaceHolderValue = value,
                DateCreated = DateTime.UtcNow,
            };

            emailPlaceholders.Add(placeholder);
        }

        await _emailPlaceholderRepository.AddRangeAsync(emailPlaceholders);

        var placeholders = await _emailPlaceholderRepository.GetByEmailOutboxIdAsync(emailOutbox.Id);
        var subject = _emailProcessor.ProcessEmailSubject(template, placeholders);
        var body = _emailProcessor.ProcessEmailBody(template, placeholders);

        var welfareEvent = new WelfareWorkTrackerEvent { Body = body, UserEmail = emailVm.ToUserEmail, Subject = subject };

        await _eventPublisher.PublishEventAsync(welfareEvent);

        return true;
    }
}
