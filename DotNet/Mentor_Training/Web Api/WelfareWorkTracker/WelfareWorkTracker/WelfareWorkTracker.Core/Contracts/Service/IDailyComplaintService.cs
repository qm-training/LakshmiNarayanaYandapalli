namespace WelfareWorkTracker.Core.Contracts.Service;
public interface IDailyComplaintService
{
    Task<DailyComplaintDto?> GetDailyComplaintByIdAsync(int id);
    Task<DailyComplaintDto?> UpdateDailyComplaintAsync(int id, DailyComplaintVm dailyComplaintVm);
    Task<List<DailyComplaintDto>> GetDailyComplaintsAsync();
    Task<DailyComplaintDto?> GetDailyComplaintByLeaderIdAsync(int leaderId);
    Task AssignDailyComplaintsAsync();
    Task<DailyComplaintDto?> GetDailyComplaintByConstituencyNameAsync(string constituencyName);
}
