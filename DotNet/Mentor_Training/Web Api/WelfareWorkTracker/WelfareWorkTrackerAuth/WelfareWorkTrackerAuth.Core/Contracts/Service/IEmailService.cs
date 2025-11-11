namespace WelfareWorkTrackerAuth.Core.Contracts.Services;

public interface IEmailService
{
    Task<bool> SendEmailAsync(EmailVm emailVm);
}
