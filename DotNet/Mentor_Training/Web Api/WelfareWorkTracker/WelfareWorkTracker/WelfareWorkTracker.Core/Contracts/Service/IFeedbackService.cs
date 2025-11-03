using WelfareWorkTracker.Core.Dtos;
using WelfareWorkTracker.Core.Models;
using WelfareWorkTracker.Core.Vms;

namespace WelfareWorkTracker.Core.Contracts.Service
{
    public interface IFeedbackService
    {
        Task<ComplaintFeedbackDto> AddFeedbackAsync(ComplaintFeedbackVm citizenFeedbackVm);
        Task<ComplaintFeedbackDto?> GetFeedbackByUserAsync(int? complaintId, int? dailyComplaintId);
        Task<List<ComplaintFeedbackDto>?> GetAllFeedbacksAsync(int? complaintId, int? dailyComplaintId);
        Task EvaluateFeedback(int complaintId);
        Task EvaluateDailyComplaintFeedback();
    }
}
