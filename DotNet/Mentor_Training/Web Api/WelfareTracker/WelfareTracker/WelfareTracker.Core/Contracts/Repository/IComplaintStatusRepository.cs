using WelfareTracker.Core.Models;

namespace WelfareTracker.Core.Contracts.Repository
{
    public interface IComplaintStatusRepository
    {
        Task<ComplaintStatus> AddComplaintStatusAsync(ComplaintStatus complaintStatus);
        Task<ComplaintStatus?> GetComplaintStatusByIdAsync(int complaintStatusId);
        Task<ComplaintStatus?> GetComplaintStatusByComplaintIdAsync(int complaintId);
        Task<bool> DeleteComplaintStatusByComplaintIdAsync(int complaintId);
    }
}
