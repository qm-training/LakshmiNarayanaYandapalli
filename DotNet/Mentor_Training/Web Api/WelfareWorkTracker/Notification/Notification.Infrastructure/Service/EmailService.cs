namespace Notification.Infrastructure.Service;
public class EmailService(IOptions<SmtpClientOptions> mailSettings, ISmtpClientWrapper smtpClientWrapper) : IEmailService
{
    private readonly IOptions<SmtpClientOptions> _mailSettings = mailSettings ?? throw new ArgumentNullException(nameof(mailSettings));
    private readonly ISmtpClientWrapper _smtpClientWrapper = smtpClientWrapper ?? throw new ArgumentNullException(nameof(smtpClientWrapper));

    public async Task<bool> SendEmailAsync(WelfareEvent welfareEvent)
    {
        try
        {
            // Sender's email address and password (for authentication)
            string senderEmail = _mailSettings.Value.SenderEmail;
            string password = _mailSettings.Value.Password;

            // Recipient's email address
            string recipientEmail = welfareEvent.UserEmail;

            // Configure SMTP client wrapper
            _smtpClientWrapper.SetCredentials(new NetworkCredential(senderEmail, password));
            _smtpClientWrapper.SetEnableSsl(true);

            // Create and send an email
            var mail = new System.Net.Mail.MailMessage(senderEmail, recipientEmail)
            {
                Subject = welfareEvent.Subject,
                Body = welfareEvent.Body,
                IsBodyHtml = true,  // Set to true if using HTML in the body
            };

            await _smtpClientWrapper.SendMailAsync(mail);
            Console.WriteLine("Email sent successfully!");
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
