namespace WelfareWorkTracker.Core.Contracts.Service;
public interface IFeedbackService
{
    Task<FeedbackDto> AddFeedbackAsync(FeedbackVm citizenFeedbackVm);
    Task<FeedbackDto?> GetFeedbackByUserAsync(int? complaintId, int? dailyComplaintId);
    Task<List<FeedbackDto>?> GetAllFeedbacksAsync(int? complaintId, int? dailyComplaintId);
    Task EvaluateFeedback(int complaintId);
    Task EvaluateDailyComplaintFeedback();
}
