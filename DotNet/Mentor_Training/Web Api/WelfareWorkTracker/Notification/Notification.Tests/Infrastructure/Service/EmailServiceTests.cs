using Microsoft.Extensions.Options;
using Notification.Core.Options;
using System.Net;
using System.Net.Mail;

namespace Notification.Tests.Infrastructure.Service;
public class EmailServiceTests
{
    private readonly Mock<IOptions<SmtpClientOptions>> _mockMailSettings;
    private readonly Mock<ISmtpClientWrapper> _mockSmtpClientWrapper;
    private readonly EmailService _emailService;
    private readonly SmtpClientOptions _smtpOptions;

    public EmailServiceTests()
    {
        _smtpOptions = new SmtpClientOptions
        {
            SenderEmail = "sender@example.com",
            Password = "senderPassword"
        };

        _mockMailSettings = new Mock<IOptions<SmtpClientOptions>>();
        _mockMailSettings.Setup(o => o.Value).Returns(_smtpOptions);

        _mockSmtpClientWrapper = new Mock<ISmtpClientWrapper>();

        _emailService = new EmailService(_mockMailSettings.Object, _mockSmtpClientWrapper.Object);
    }

    [Fact]
    public async Task SendEmailAsync_WithValidEvent_ReturnsTrueAndSendsMail()
    {
        // Arrange
        var welfareEvent = CreateWelfareWorkTrackerEvent();

        _mockSmtpClientWrapper
            .Setup(s => s.SendMailAsync(It.IsAny<MailMessage>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _emailService.SendEmailAsync(welfareEvent);

        // Assert
        Assert.True(result);

        _mockSmtpClientWrapper.Verify(s => s.SetCredentials(
            It.Is<NetworkCredential>(c =>
                c.UserName == _smtpOptions.SenderEmail &&
                c.Password == _smtpOptions.Password)),
            Times.Once);

        _mockSmtpClientWrapper.Verify(s => s.SetEnableSsl(true), Times.Once);

        _mockSmtpClientWrapper.Verify(s => s.SendMailAsync(
            It.Is<MailMessage>(m =>
                m.From.Address == _smtpOptions.SenderEmail &&
                m.To.Single().Address == welfareEvent.UserEmail &&
                m.Subject == welfareEvent.Subject &&
                m.Body == welfareEvent.Body &&
                m.IsBodyHtml)),
            Times.Once);
    }

    [Fact]
    public async Task SendEmailAsync_WhenSendMailAsyncThrowsException_ReturnsFalse()
    {
        // Arrange
        var welfareEvent = CreateWelfareWorkTrackerEvent();

        _mockSmtpClientWrapper
            .Setup(s => s.SendMailAsync(It.IsAny<MailMessage>()))
            .ThrowsAsync(new Exception("SMTP failure"));

        // Act
        var result = await _emailService.SendEmailAsync(welfareEvent);

        // Assert
        Assert.False(result);

        _mockSmtpClientWrapper.Verify(s => s.SetCredentials(It.IsAny<NetworkCredential>()), Times.Once);
        _mockSmtpClientWrapper.Verify(s => s.SetEnableSsl(true), Times.Once);
        _mockSmtpClientWrapper.Verify(s => s.SendMailAsync(It.IsAny<MailMessage>()), Times.Once);
    }

    [Fact]
    public void Constructor_WithNullMailSettings_ThrowsArgumentNullException()
    {
        // Arrange
        IOptions<SmtpClientOptions> mailSettings = null!;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new EmailService(mailSettings, _mockSmtpClientWrapper.Object));

        // Assert
        Assert.Equal("mailSettings", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullSmtpClientWrapper_ThrowsArgumentNullException()
    {
        // Arrange
        ISmtpClientWrapper smtpClientWrapper = null!;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new EmailService(_mockMailSettings.Object, smtpClientWrapper));

        // Assert
        Assert.Equal("smtpClientWrapper", exception.ParamName);
    }

    private static WelfareWorkTrackerEvent CreateWelfareWorkTrackerEvent()
    {
        return new WelfareWorkTrackerEvent
        {
            UserEmail = "user@example.com",
            Subject = "Test Subject",
            Body = "<p>Test Body</p>"
        };
    }
}
