using WelfareWorkTracker.Core.Dtos;
using WelfareWorkTracker.Core.Vms;

namespace WelfareWorkTracker.Core.Contracts.Service
{
    public interface IComplaintStatusService
    {
        Task<ComplaintStatusDto> AddComplaintStatusByAdminRep(ComplaintStatusAdminRepVm complaintStatusAdminRepVm);
        Task<ComplaintStatusDto> AddComplaintStatusByAdmin(ComplaintStatusAdminVm complaintStatusAdminVm);
        Task<ComplaintStatusDto> AddComplaintStatusByLeader(ComplaintStatusLeaderVm complaintStatusLeaderVm);
        Task<ComplaintStatusDto> GetComplaintStatusByIdAsync(int complaintStatusId);
        Task<ComplaintStatusDto> GetComplaintStatusByComplaintId(int complaintId);
        Task<List<ComplaintStatusDto>> GetComplaintStatusHistoryAsync(int complaintId);
    }
}
