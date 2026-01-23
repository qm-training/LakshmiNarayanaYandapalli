namespace Notification.Core.Contracts.Service;
public interface ISmtpClientWrapper
{
    Task SendMailAsync(MailMessage mailMessage);
    void SetCredentials(NetworkCredential credentials);
    void SetEnableSsl(bool enableSsl);
}