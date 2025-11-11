namespace WelfareWorkTrackerAuth.Core.Contracts.Services;

public interface IEmailProcessorService
{
    string ProcessEmailBody(EmailTemplate template, List<EmailPlaceholder> placeholderData);
    string ProcessEmailSubject(EmailTemplate template, List<EmailPlaceholder> placeholderData);
}

