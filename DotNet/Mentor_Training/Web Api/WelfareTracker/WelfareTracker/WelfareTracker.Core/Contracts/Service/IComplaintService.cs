using WelfareTracker.Core.Dtos;
using WelfareTracker.Core.Vms;

namespace WelfareTracker.Core.Contracts.Service
{
    public interface IComplaintService
    {
        Task<ComplaintDto> AddComplaintAsync(ComplaintVm complaintVm);
        Task<ComplaintDto?> UpdateComplaintAsync(int complaintId, ComplaintVm complaintVm);
        Task<ComplaintDto> GetComplaintByComplaintIdAsync(int complaintId);
        Task<List<ComplaintDto>?> GetComplaintsByCitizenIdAsync(int citizenId);
    }
}
