using WelfareTracker.Core.Dtos;
using WelfareTracker.Core.Vms;

namespace WelfareTracker.Core.Contracts.Service;
public interface IComplaintStatusService
{
    Task<ComplaintDto?> AddComplaintStatusByAdminRep(AdminRepStatusVm adminRepStatusVm);
}
