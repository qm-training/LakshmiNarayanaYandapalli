namespace Notification.Infrastructure.Service;
public class EmailReceiverService(IEmailService emailService) : IConsumer<WelfareEvent>
{
    private readonly IEmailService _emailService = emailService;

    public async Task Consume(ConsumeContext<WelfareEvent> context)
    {
        try
        {
            var emailEvent = context.Message;
            await _emailService.SendEmailAsync(emailEvent);
        }
        catch (Exception ex) when (ex.Message != null)
        {
        }
    }
}
