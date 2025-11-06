using System.Net;
using WelfareWorkTracker.Core.Contracts.Repository;
using WelfareWorkTracker.Core.Contracts.Service;
using WelfareWorkTracker.Core.Exceptions;
using WelfareWorkTracker.Core.Models;
using WelfareWorkTracker.Core.Vms;

namespace WelfareWorkTracker.Infrastructure.Service
{
    public class EmailService(IEmailProcessorService emailProcessor, IEmailTemplateRepository emailTemplateRepository, IEmailPlaceholderRepository emailPlaceholderRepository, IEmailOutboxRepository emailHistoryRepository, IEventPublisher eventPublisher) : IEmailService
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
            var emailHistory = new EmailOutbox
            {
                EmailTemplateId = template.Id,
                ToEmail = emailVm.ToUserEmail,
                SentAt = DateTime.UtcNow,
                FromEmail = emailVm.FromUserEmail,
            };

            await _emailHistoryRepository.AddAsync(emailHistory);

            var emailPlaceholders = new List<EmailPlaceholder>();

            // 3. Insert placeholder data
            foreach (var (key, value) in emailVm.Payload)
            {
                var placeholder = new EmailPlaceholder
                {
                    EmailHistoryId = emailHistory.Id,
                    PlaceHolderKey = key,
                    PlaceHolderValue = value,
                    DateCreated = DateTime.UtcNow,
                };

                emailPlaceholders.Add(placeholder); // Add to the list
            }

            await _emailPlaceholderRepository.AddRangeAsync(emailPlaceholders);



            // 4. Process the email subject and body
            var placeholders = await _emailPlaceholderRepository.GetByEmailHistoryIdAsync(emailHistory.Id);
            var subject = _emailProcessor.ProcessEmailSubject(template, placeholders);
            var body = _emailProcessor.ProcessEmailBody(template, placeholders);

            var welfareEvent = new WelfareEvent { Body = body, UserEmail = emailVm.ToUserEmail, Subject = subject };

            await _eventPublisher.PublishEventAsync<WelfareEvent>(welfareEvent);

            return true;
        }
    }
}
