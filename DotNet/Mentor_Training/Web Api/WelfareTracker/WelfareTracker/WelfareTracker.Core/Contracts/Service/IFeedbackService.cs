using WelfareTracker.Core.Dtos;
using WelfareTracker.Core.Vms;

namespace WelfareTracker.Core.Contracts.Service
{
    public interface IFeedbackService
    {
        Task<FeedbackDto> AddFeedbackAsync(FeedbackVm feedbackVm);
        Task<FeedbackDto?> GetFeedbackByUserAsync(int? complaintId, int? dailyComplaintId);
        Task<List<FeedbackDto>?> GetAllFeedbacksAsync(int? complaintId, int? dailyComplaintId);
        Task EvaluateFeedback(int complaintId);
        Task EvaluateDailyComplaintFeedback();
    }
}
