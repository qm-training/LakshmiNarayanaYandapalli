namespace WelfareWorkTracker.Core.Contracts.Repository;
public interface IDailyComplaintStatusRepository
{
    Task<DailyComplaintStatus> AddDailyComplaintStatusAsync(DailyComplaintStatus dailyComplaintStatus);
    Task<DailyComplaintStatus?> GetDailyComplaintStatusAsync(int? dailyComplaintId = null, int? dailyComplaintStatusId = null);
    Task<List<DailyComplaintStatus?>> GetPendingDailyComplaintsAsync();
    Task<List<DailyComplaintStatus>> GetUnresolvedComplaintsOfLeader(int leaderId, Status status);
}
