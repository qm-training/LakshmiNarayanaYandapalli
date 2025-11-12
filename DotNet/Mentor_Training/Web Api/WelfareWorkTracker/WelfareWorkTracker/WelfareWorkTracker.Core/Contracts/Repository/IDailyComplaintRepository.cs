namespace WelfareWorkTracker.Core.Contracts.Repository;
public interface IDailyComplaintRepository
{
    Task<DailyComplaint> AddDailyComplaintAsync(DailyComplaint dailyComplaint);
    Task<DailyComplaint?> GetDailyComplaintByIdAsync(int id);
    Task<DailyComplaint> UpdateDailyComplaintAsync(DailyComplaint dailyComplaint);
    Task<List<DailyComplaint>> GetDailyComplaintsAsync();
    Task<DailyComplaint?> GetDailyComplaintByLeaderIdAsync(int leaderId);
    Task<List<DailyComplaint>> GetTodaysDailyComplaintsAsync();
    Task<List<DailyComplaint>> GetDailyComplaintsForLeaderByLeaderIdAsync(int leaderId);
    Task<DailyComplaint?> GetDailyComplaintsByConstituencyNameAsync(string constituencyName);
}
