using WelfareTracker.Core.Models;

namespace WelfareTracker.Core.Contracts.Repository
{
    public interface IDailyComplaintStatusRepository
    {
        Task<DailyComplaintStatus> AddDailyComplaintStatusAsync(DailyComplaintStatus dailyComplaintStatus);
        Task<DailyComplaintStatus?> GetDailyComplaintStatusByDailyComplaintStatusIdAsync(int dailyComplaintStatusId);
        Task<DailyComplaintStatus?> GetDailyComplaintStatusByDailyComplaintIdAsync(int dailyComplaintId);
    }
}
