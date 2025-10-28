using WelfareWorkTracker.Core.Models;

namespace WelfareWorkTracker.Core.Contracts.Repository
{
    public interface IDailyComplaintRepository
    {
        Task<DailyComplaint> AddDailyComplaintAsync(DailyComplaint dailyComplaint);
        Task<DailyComplaint?> GetDailyComplaintByIdAsync(int dailyComplaintId);
        Task<DailyComplaint> UpdateDailyComplaintAsync(DailyComplaint dailyComplaint);
        Task<DailyComplaint?> GetDailyComplaintForLeaderByLeaderIdAsync(int leaderId);
        Task<List<DailyComplaint>> GetTodaysDailyComplaintsAsync();
        Task<List<DailyComplaint>> GetAllDailyComplaintsForLeaderAsync(int leaderId);
        Task<List<DailyComplaint>> GetAllDailyComplaintsAsync();
    }
}
