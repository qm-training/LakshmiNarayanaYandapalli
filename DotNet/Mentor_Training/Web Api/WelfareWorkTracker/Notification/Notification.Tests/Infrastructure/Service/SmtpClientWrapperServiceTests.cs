namespace Notification.Tests.Infrastructure.Service;
public class SmtpClientWrapperServiceTests
{
    private readonly SmtpClientOptions _smtpOptions;
    private readonly Mock<IOptions<SmtpClientOptions>> _mockOptions;

    public SmtpClientWrapperServiceTests()
    {
        _smtpOptions = new SmtpClientOptions
        {
            SmtpServer = "localhost",
            Port = 25,
            SenderEmail = "sender@example.com",
            Password = "password123",
            EnableSsl = true
        };

        _mockOptions = new Mock<IOptions<SmtpClientOptions>>();
        _mockOptions.Setup(o => o.Value).Returns(_smtpOptions);
    }

    [Fact]
    public void Constructor_WithValidOptions_SetsSmtpClientPropertiesCorrectly()
    {
        // Arrange & Act
        var wrapper = new SmtpClientWrapperService(_mockOptions.Object);

        // Assert
        var smtpClient = GetInternalSmtpClient(wrapper);

        Assert.Equal(_smtpOptions.SmtpServer, smtpClient.Host);
        Assert.Equal(_smtpOptions.Port, smtpClient.Port);

        var credentials = smtpClient.Credentials as NetworkCredential;
        Assert.NotNull(credentials);
        Assert.Equal(_smtpOptions.SenderEmail, credentials!.UserName);
        Assert.Equal(_smtpOptions.Password, credentials.Password);

        Assert.True(smtpClient.EnableSsl);
    }

    [Fact]
    public void SetCredentials_WithValidCredentials_UpdatesSmtpClientCredentials()
    {
        // Arrange
        var wrapper = new SmtpClientWrapperService(_mockOptions.Object);
        var newCredentials = new NetworkCredential("newuser@example.com", "newpassword");

        // Act
        wrapper.SetCredentials(newCredentials);

        // Assert
        var smtpClient = GetInternalSmtpClient(wrapper);
        var creds = smtpClient.Credentials as NetworkCredential;

        Assert.Equal("newuser@example.com", creds!.UserName);
        Assert.Equal("newpassword", creds.Password);
    }

    [Fact]
    public void SetEnableSsl_WithBooleanValue_UpdatesSmtpEnableSsl()
    {
        // Arrange
        var wrapper = new SmtpClientWrapperService(_mockOptions.Object);

        // Act
        wrapper.SetEnableSsl(false);

        // Assert
        var smtpClient = GetInternalSmtpClient(wrapper);
        Assert.False(smtpClient.EnableSsl);

        // Act again
        wrapper.SetEnableSsl(true);

        // Assert
        Assert.True(smtpClient.EnableSsl);
    }

    [Fact]
    public async Task SendMailAsync_WithInvalidSmtpServer_ThrowsException()
    {
        // Arrange
        var wrapper = new SmtpClientWrapperService(_mockOptions.Object);
        var mailMessage = new MailMessage("sender@example.com", "recipient@example.com")
        {
            Subject = "Test",
            Body = "Body"
        };

        // Act
        var exception = await Record.ExceptionAsync(() => wrapper.SendMailAsync(mailMessage));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<SmtpException>(exception);
    }

    private static SmtpClient GetInternalSmtpClient(SmtpClientWrapperService wrapper)
    {
        var field = typeof(SmtpClientWrapperService)
            .GetField("_smtpClient", BindingFlags.NonPublic | BindingFlags.Instance);

        return (SmtpClient)field!.GetValue(wrapper)!;
    }
}
