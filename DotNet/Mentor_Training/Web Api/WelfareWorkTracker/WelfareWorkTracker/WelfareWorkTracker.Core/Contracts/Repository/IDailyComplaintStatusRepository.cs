using WelfareWorkTracker.Core.Enums;
using WelfareWorkTracker.Core.Models;

namespace WelfareWorkTracker.Core.Contracts.Repository
{
    public interface IDailyComplaintStatusRepository
    {
        Task<DailyComplaintStatus> AddDailyComplaintStatusAsync(DailyComplaintStatus dailyComplaintStatus);
        Task<DailyComplaintStatus?> GetDailyComplaintStatusByDailyComplaintStatusIdAsync(int dailyComplaintStatusId);
        Task<DailyComplaintStatus?> GetDailyComplaintStatusByDailyComplaintIdAsync(int dailyComplaintId);
        Task<List<DailyComplaintStatus?>> GetPendingDailyComplaintsAsync();
        Task<List<DailyComplaintStatus>> GetUnresolvedComplaintsOfLeader(int leaderId, int status);
    }
}
