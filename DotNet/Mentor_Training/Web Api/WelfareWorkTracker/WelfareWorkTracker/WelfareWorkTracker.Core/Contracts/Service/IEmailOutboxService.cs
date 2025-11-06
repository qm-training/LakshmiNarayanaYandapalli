using WelfareWorkTracker.Core.Dtos;

namespace WelfareWorkTracker.Core.Contracts.Service
{
    public interface IEmailOutboxService
    {
        Task<EmailOutboxDto> GetEmailOutboxByIdAsync(int emailOutboxId);
    }
}
