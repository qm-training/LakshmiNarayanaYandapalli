using WelfareWorkTracker.Core.Dtos;
using WelfareWorkTracker.Core.Models;
using WelfareWorkTracker.Core.Vms;

namespace WelfareWorkTracker.Core.Contracts.Service
{
    public interface IDailyComplaintStatusService
    {
        Task<DailyComplaintStatusDto> AddDailyComplaintStatusByLeader(DailyComplaintStatusVm dailyComplaintStatusVm);
        Task CheckDailyComplaintStatusAsync();
    }
}
