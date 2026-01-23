namespace Notification.Tests.Infrastructure.Service;

public class EmailReceiverServiceTests
{
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly EmailReceiverService _emailReceiverService;

    public EmailReceiverServiceTests()
    {
        _mockEmailService = new Mock<IEmailService>();
        _emailReceiverService = new EmailReceiverService(_mockEmailService.Object);
    }

    [Fact]
    public async Task Consume_WithValidEvent_CallsEmailServiceSendEmailAsyncOnce()
    {
        // Arrange
        var welfareEvent = CreateWelfareWorkTrackerEvent();
        var mockContext = new Mock<ConsumeContext<WelfareWorkTrackerEvent>>();
        mockContext.SetupGet(c => c.Message).Returns(welfareEvent);

        _mockEmailService
            .Setup(s => s.SendEmailAsync(welfareEvent))
            .ReturnsAsync(true);

        // Act
        await _emailReceiverService.Consume(mockContext.Object);

        // Assert
        _mockEmailService.Verify(s => s.SendEmailAsync(welfareEvent), Times.Once);
    }

    [Fact]
    public async Task Consume_WhenEmailServiceThrowsException_DoesNotPropagateException()
    {
        // Arrange
        var welfareEvent = CreateWelfareWorkTrackerEvent();
        var mockContext = new Mock<ConsumeContext<WelfareWorkTrackerEvent>>();
        mockContext.SetupGet(c => c.Message).Returns(welfareEvent);

        _mockEmailService
            .Setup(s => s.SendEmailAsync(welfareEvent))
            .ThrowsAsync(new Exception("Error sending email"));

        // Act
        var exception = await Record.ExceptionAsync(() => _emailReceiverService.Consume(mockContext.Object));

        // Assert
        Assert.Null(exception);
        _mockEmailService.Verify(s => s.SendEmailAsync(welfareEvent), Times.Once);
    }

    private static WelfareWorkTrackerEvent CreateWelfareWorkTrackerEvent()
    {
        return new WelfareWorkTrackerEvent
        {
            UserEmail = "user@example.com",
            Subject = "Test Subject",
            Body = "Test Body"
        };
    }
}
