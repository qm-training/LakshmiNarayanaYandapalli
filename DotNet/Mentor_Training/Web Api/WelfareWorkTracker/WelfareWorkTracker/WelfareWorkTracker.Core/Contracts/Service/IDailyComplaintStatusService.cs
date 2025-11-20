namespace WelfareWorkTracker.Core.Contracts.Service;
public interface IDailyComplaintStatusService
{
    Task<DailyComplaintStatusDto?> UpdateDailyComplaintStatusByLeaderAsync(DailyComplaintStatusVm dailyComplaintStatusVm);
    public Task CheckDailyComplaintStatusAsync();
}
