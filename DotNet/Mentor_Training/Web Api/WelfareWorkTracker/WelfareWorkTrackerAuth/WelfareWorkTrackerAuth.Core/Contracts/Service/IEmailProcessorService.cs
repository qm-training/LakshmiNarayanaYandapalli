namespace WelfareWorkTrackerAuth.Core.Contracts.Service;

public interface IEmailProcessorService
{
    string ProcessEmailBody(EmailTemplate template, List<EmailPlaceholder> placeholderData);
    string ProcessEmailSubject(EmailTemplate template, List<EmailPlaceholder> placeholderData);
}

