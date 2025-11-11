namespace WelfareWorkTracker.Infrastructure.Service;
public class EmailProcessorService : IEmailProcessorService
{
    public string ProcessEmailBody(EmailTemplate template, List<EmailPlaceholder> placeholderData)
    {
        return ReplacePlaceholders(template.Body, placeholderData);
    }

    public string ProcessEmailSubject(EmailTemplate template, List<EmailPlaceholder> placeholderData)
    {
        return ReplacePlaceholders(template.Subject, placeholderData);
    }

    private static string ReplacePlaceholders(string templateText, IEnumerable<EmailPlaceholder> placeholderData)
    {
        if (string.IsNullOrEmpty(templateText))
            return string.Empty;

        foreach (var prop in placeholderData)
        {
            templateText = templateText.Replace($"{{{{{prop.PlaceHolderKey}}}}}", prop.PlaceHolderValue);
        }

        return templateText;
    }
}
