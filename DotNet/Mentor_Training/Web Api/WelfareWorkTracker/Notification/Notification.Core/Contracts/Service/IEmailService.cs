namespace Notification.Core.Contracts.Service;
public interface IEmailService
{
    Task<bool> SendEmailAsync(WelfareWorkTrackerEvent welfareEvent);
}