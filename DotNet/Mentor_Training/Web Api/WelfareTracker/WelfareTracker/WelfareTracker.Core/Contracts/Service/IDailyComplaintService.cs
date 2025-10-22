using WelfareTracker.Core.Dtos;
using WelfareTracker.Core.Vms;

namespace WelfareTracker.Core.Contracts.Service
{
    public interface IDailyComplaintService
    {
        Task AssignDailyComplaintsToLeadersAsync();
        Task<DailyComplaintDto?> UpdateDailyComplaintAsync(int dailyComplaintId, DailyComplaintVm dailyComplaintVm);
        Task<DailyComplaintDto?> GetDailyComplaintByIdAsync(int dailyComplaintId);
        Task<DailyComplaintDto?> GetDailyComplaintByLeaderIdAsync(int leaderId);
        Task<List<DailyComplaintDto>?> GetTodaysDailyComplaintsAsync();
    }
}
