namespace Notification.Infrastructure.Service;
public class EmailReceiverService(IEmailService emailService) : IConsumer<WelfareEvent>
{
    private readonly IEmailService _emailService = emailService;

    public async Task Consume(ConsumeContext<WelfareEvent> context)
    {
        try
        {
            var emailEvent = context.Message;
            Log.Information("sarted mailing process to user and order with Id and htmlContent {Message}", context.Message);
            await _emailService.SendEmailAsync(emailEvent);
        }
        catch (Exception ex) when (ex.Message != null)
        {
            Log.Error(ex, "An unexpected error occured while receiving message from queue and sending email to customer");
        }
    }
}
