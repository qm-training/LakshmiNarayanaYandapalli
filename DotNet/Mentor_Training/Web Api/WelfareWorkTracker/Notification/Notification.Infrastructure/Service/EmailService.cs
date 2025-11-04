namespace Notification.Infrastructure.Service;
public class EmailService(IOptions<SmtpClientOptions> mailSettings, ISmtpClientWrapper smtpClientWrapper) : IEmailService
{
    private readonly IOptions<SmtpClientOptions> _mailSettings = mailSettings ?? throw new ArgumentNullException(nameof(mailSettings));
    private readonly ISmtpClientWrapper _smtpClientWrapper = smtpClientWrapper ?? throw new ArgumentNullException(nameof(smtpClientWrapper));

    public async Task<bool> SendEmailAsync(WelfareEvent welfareEvent)
    {
        try
        {
            string senderEmail = _mailSettings.Value.SenderEmail;
            string password = _mailSettings.Value.Password;

            string recipientEmail = welfareEvent.UserEmail;

            _smtpClientWrapper.SetCredentials(new NetworkCredential(senderEmail, password));
            _smtpClientWrapper.SetEnableSsl(true);

            var mail = new MailMessage(senderEmail, recipientEmail)
            {
                Subject = welfareEvent.Subject,
                Body = welfareEvent.Body,
                IsBodyHtml = true
            };

            await _smtpClientWrapper.SendMailAsync(mail);
            Log.Information("Email sent successfully!");
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to send email to: {EmailToId}", welfareEvent.UserEmail);
            return false;
        }
    }
}
