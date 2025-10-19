using WelfareTracker.Core.Models;

namespace WelfareTracker.Core.Contracts.Repository
{
    public interface IComplaintRepository
    {
        Task<Complaint> AddComplaintAsync(Complaint complaint);
        Task<Complaint> UpdateComplaintAsync(Complaint complaint);
        Task<Complaint?> GetComplaintByIdAsync(int complaintId);
        Task<List<Complaint>> GetComplaintsByCitizenIdAsync(int citizenId);
        Task<Complaint> DeleteComplaintAsync(Complaint complaint);
        Task<List<Complaint>> GetLeaderComplaintsByLeaderIdAsync(int leaderId);
        Task<List<Complaint>> GetAllReferredComplaintsAsync(int complaintId);
    }
}
