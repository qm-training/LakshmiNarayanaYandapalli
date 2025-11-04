namespace Notification.Infrastructure.Service;
public class SmtpClientWrapperService : ISmtpClientWrapper
{
    private readonly SmtpClient _smtpClient;

    public SmtpClientWrapperService(IOptions<SmtpClientOptions> options)
    {
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
        _smtpClient.Credentials = credentials;
    }

    public void SetEnableSsl(bool enableSsl)
    {
        _smtpClient.EnableSsl = enableSsl;
    }
}
