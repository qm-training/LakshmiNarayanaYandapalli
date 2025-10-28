using WelfareWorkTracker.Core.Dtos;
using WelfareWorkTracker.Core.Vms;

namespace WelfareWorkTracker.Core.Contracts.Service
{
    public interface IComplaintService
    {
        Task<ComplaintDto> AddComplaintAsync(ComplaintVm complaintVm);
        Task<ComplaintDto> UpdateComplaintAsync(int complaintId, ComplaintVm complaintVm);
        Task<bool> DeleteComplaintByIdAsync(int complaintId);
        Task<ComplaintDto> GetComplaintByIdAsync(int complaintId);
        Task<List<ComplaintDto>> GetComplaintsByUserAsync(int userId);
        Task<List<ComplaintDto>> GetRecentComplaintsInConstituencyAsync(int constituencyId);
        Task<List<ComplaintDto>> GetComplaintsForLeaderAsync(int leaderId);
        Task<bool> UpdateStatusOfAllReferredComplaintsAsync(int complaintId);
        Task CheckComplaintsForLeaderApprovalAsync();
        Task<List<ComplaintDto>> GetComplaintsForAdminRepAsync(GetComplaintsVm getComplaintsVm);
        Task<List<ComplaintDto>> GetComplaintsByConstituencyAndStatusAsync(GetComplaintsVm getComplaintsVm);
    }
}
