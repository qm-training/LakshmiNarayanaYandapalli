using WelfareTracker.Core.Models;

namespace WelfareTracker.Core.Contracts.Repository
{
    public interface IComplaintRepository
    {
        Task<Complaint> AddComplaintAsync(Complaint complaint);
        Task<Complaint> UpdateComplaintAsync(Complaint complaint);
        Task<Complaint?> GetComplaintByIdAsync(int complaintId);
    }
}
