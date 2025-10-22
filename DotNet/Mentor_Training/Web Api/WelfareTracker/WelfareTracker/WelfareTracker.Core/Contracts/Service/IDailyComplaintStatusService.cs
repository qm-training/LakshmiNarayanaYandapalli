using WelfareTracker.Core.Dtos;
using WelfareTracker.Core.Vms;

namespace WelfareTracker.Core.Contracts.Service
{
    public interface IDailyComplaintStatusService
    {
        Task<DailyComplaintStatusDto> AddDailyComplaintStatusByLeader(DailyComplaintStatusVm dailyComplaintStatusVm);
    }
}
