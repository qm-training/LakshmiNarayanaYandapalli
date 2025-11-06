using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WelfareWorkTracker.Core.Contracts.Repository;
using WelfareWorkTracker.Core.Contracts.Service;
using WelfareWorkTracker.Core.Dtos;

namespace WelfareWorkTracker.Infrastructure.Service
{
    public class EmailOutboxService(IEmailTemplateRepository emailTemplateRepository,
                                        IEmailOutboxRepository emailOutboxRepository,
                                        IEmailPlaceholderRepository emailPlaceholderRepository,
                                        IEmailProcessorService emailProcessorService) : IEmailOutboxService
    {
        private readonly IEmailTemplateRepository _emailTemplateRepository = emailTemplateRepository;
        private readonly IEmailOutboxRepository _emailOutboxRepository = emailOutboxRepository;
        private readonly IEmailPlaceholderRepository _emailPlaceholderRepository = emailPlaceholderRepository;
        private readonly IEmailProcessorService _emailProcessorService = emailProcessorService;

        public async Task<EmailOutboxDto> GetEmailOutboxByIdAsync(int emailOutboxId)
        {
            var emailOutbox = await _emailOutboxRepository.GetByIdAsync(emailOutboxId);

            var emailTemplate = await _emailTemplateRepository.GetByIdAsync(emailOutbox.EmailTemplateId);

            var emailPlaceholders = await _emailPlaceholderRepository.GetByEmailHistoryIdAsync(emailOutboxId);

            var payload = new Dictionary<string, string>();

            foreach (var emailPlaceholder in emailPlaceholders)
            {
                payload.Add(emailPlaceholder.PlaceHolderKey, emailPlaceholder.PlaceHolderValue);

            }

            var subject = _emailProcessorService.ProcessEmailSubject(emailTemplate, emailPlaceholders);
            var body = _emailProcessorService.ProcessEmailBody(emailTemplate, emailPlaceholders);

            return new EmailOutboxDto
            {
                Body = body,
                Subject = subject,
                EmailOutboxId = emailOutboxId,
                FromUserMail = emailOutbox.FromEmail,
                ToUserMail = emailOutbox.ToEmail,

            };
        }
    }
}
