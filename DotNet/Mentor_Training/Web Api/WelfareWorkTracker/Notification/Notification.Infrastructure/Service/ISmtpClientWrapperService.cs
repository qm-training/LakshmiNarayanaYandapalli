namespace Notification.Infrastructure.Service;
public class SmtpClientWrapperService : ISmtpClientWrapper
{
    private readonly SmtpClient _smtpClient;

    public SmtpClientWrapperService(IOptions<SmtpClientOptions> options)
    {
        // Declare options as a local variable
        var smtpOptions = options.Value;

        _smtpClient = new SmtpClient(smtpOptions.SmtpServer, smtpOptions.Port)
        {
            Credentials = new NetworkCredential(smtpOptions.SenderEmail, smtpOptions.Password),
            EnableSsl = smtpOptions.EnableSsl
        };
    }

    public Task SendMailAsync(MailMessage mailMessage)
    {
        return _smtpClient.SendMailAsync(mailMessage);
    }

    public void SetCredentials(NetworkCredential credentials)
    {
        // Update the credentials of the SmtpClient
        _smtpClient.Credentials = credentials;
    }

    public void SetEnableSsl(bool enableSsl)
    {
        // Update the SSL setting of the SmtpClient
        _smtpClient.EnableSsl = enableSsl;
    }
}
