namespace Notification.Core.Options;
public class SmtpClientOptions
{
    public string SmtpServer { get; set; } = "SmtpClientOptions";
    public int Port { get; set; }
    public string SenderEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool EnableSsl { get; set; }
}