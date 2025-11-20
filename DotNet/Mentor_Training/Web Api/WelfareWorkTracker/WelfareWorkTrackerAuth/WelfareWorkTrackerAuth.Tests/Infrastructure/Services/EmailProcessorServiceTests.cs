namespace WelfareWorkTrackerAuth.Tests.Infrastructure.Services;
public class EmailProcessorServiceTests
{
    private readonly EmailProcessorService _emailProcessorService;

    public EmailProcessorServiceTests()
    {
        _emailProcessorService = new EmailProcessorService();
    }

    [Fact]
    public void ProcessEmailBody_WithValidTemplateAndPlaceholders_ReplacesAllPlaceholders()
    {
        // Arrange
        var template = CreateEmailTemplate(
            subject: "Subject {{UserName}}",
            body: "Hello {{UserName}}, your complaint {{ComplaintId}} is {{Status}}."
        );

        var placeholders = new List<EmailPlaceholder>
        {
            CreateEmailPlaceholder("UserName", "Narayana"),
            CreateEmailPlaceholder("ComplaintId", "12345"),
            CreateEmailPlaceholder("Status", "Resolved")
        };

        var expectedBody = "Hello Narayana, your complaint 12345 is Resolved.";

        // Act
        var result = _emailProcessorService.ProcessEmailBody(template, placeholders);

        // Assert
        Assert.Equal(expectedBody, result);
    }

    [Fact]
    public void ProcessEmailBody_WithNullOrEmptyBody_ReturnsEmptyString()
    {
        // Arrange
        var template = CreateEmailTemplate(
            subject: "Any Subject",
            body: null
        );

        var placeholders = new List<EmailPlaceholder>
        {
            CreateEmailPlaceholder("UserName", "Narayana")
        };

        // Act
        var result = _emailProcessorService.ProcessEmailBody(template, placeholders);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ProcessEmailBody_WithNoMatchingPlaceholders_ReturnsOriginalBody()
    {
        // Arrange
        var template = CreateEmailTemplate(
            subject: "Any Subject",
            body: "Hello {{UserName}}."
        );

        var placeholders = new List<EmailPlaceholder>
        {
            CreateEmailPlaceholder("DifferentKey", "Value")
        };

        var expectedBody = "Hello {{UserName}}.";

        // Act
        var result = _emailProcessorService.ProcessEmailBody(template, placeholders);

        // Assert
        Assert.Equal(expectedBody, result);
    }

    [Fact]
    public void ProcessEmailSubject_WithValidTemplateAndPlaceholders_ReplacesAllPlaceholders()
    {
        // Arrange
        var template = CreateEmailTemplate(
            subject: "Complaint {{ComplaintId}} for {{UserName}}",
            body: "Body"
        );

        var placeholders = new List<EmailPlaceholder>
        {
            CreateEmailPlaceholder("ComplaintId", "98765"),
            CreateEmailPlaceholder("UserName", "Narayana")
        };

        var expectedSubject = "Complaint 98765 for Narayana";

        // Act
        var result = _emailProcessorService.ProcessEmailSubject(template, placeholders);

        // Assert
        Assert.Equal(expectedSubject, result);
    }

    [Fact]
    public void ProcessEmailSubject_WithNullOrEmptySubject_ReturnsEmptyString()
    {
        // Arrange
        var template = CreateEmailTemplate(
            subject: null,
            body: "Body"
        );

        var placeholders = new List<EmailPlaceholder>
        {
            CreateEmailPlaceholder("UserName", "Narayana")
        };

        // Act
        var result = _emailProcessorService.ProcessEmailSubject(template, placeholders);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ProcessEmailSubject_WithRepeatedPlaceholder_ReplacesAllOccurrences()
    {
        // Arrange
        var template = CreateEmailTemplate(
            subject: "Hello {{UserName}}, welcome {{UserName}}!",
            body: "Body"
        );

        var placeholders = new List<EmailPlaceholder>
        {
            CreateEmailPlaceholder("UserName", "Narayana")
        };

        var expectedSubject = "Hello Narayana, welcome Narayana!";

        // Act
        var result = _emailProcessorService.ProcessEmailSubject(template, placeholders);

        // Assert
        Assert.Equal(expectedSubject, result);
    }

    private static EmailTemplate CreateEmailTemplate(string? subject, string? body)
    {
        return new EmailTemplate
        {
            Id = 1,
            Name = "Test Template",
            Subject = subject ?? string.Empty,
            Body = body ?? string.Empty,
            IsActive = true,
            DateCreated = DateTime.UtcNow,
            CreatedBy = 1
        };
    }

    private static EmailPlaceholder CreateEmailPlaceholder(string key, string value)
    {
        return new EmailPlaceholder
        {
            Id = 1,
            EmailOutboxId = 1,
            PlaceHolderKey = key,
            PlaceHolderValue = value,
            DateCreated = DateTime.UtcNow
        };
    }
}
