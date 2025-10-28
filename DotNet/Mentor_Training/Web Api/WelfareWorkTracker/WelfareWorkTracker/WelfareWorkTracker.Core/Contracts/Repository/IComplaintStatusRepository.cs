using WelfareWorkTracker.Core.Models;

namespace WelfareWorkTracker.Core.Contracts.Repository
{
    public interface IComplaintStatusRepository
    {
        Task<ComplaintStatus> AddComplaintStatusAsync(ComplaintStatus complaintStatus);
        Task<ComplaintStatus?> GetComplaintStatusByIdAsync(int complaintStatusId);
        Task<ComplaintStatus?> GetComplaintStatusByComplaintIdAsync(int complaintId);
        Task<bool> DeleteComplaintStatusByComplaintIdAsync(int complaintId);
        Task<List<ComplaintStatus>> GetUnresolvedComplaintsOfLeader(int leaderId, int status);
        Task<List<ComplaintStatus>> GetComplaintStatusHistoryByComplaintIdAsync(int complaintId);
    }
}
