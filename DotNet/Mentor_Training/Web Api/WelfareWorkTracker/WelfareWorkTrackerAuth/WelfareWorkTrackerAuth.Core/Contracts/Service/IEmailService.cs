namespace WelfareWorkTrackerAuth.Core.Contracts.Service;

public interface IEmailService
{
    Task<bool> SendEmailAsync(EmailVm emailVm);
}
