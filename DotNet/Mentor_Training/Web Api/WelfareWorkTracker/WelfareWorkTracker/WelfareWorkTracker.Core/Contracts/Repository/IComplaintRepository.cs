using WelfareWorkTracker.Core.Models;

namespace WelfareWorkTracker.Core.Contracts.Repository
{
    public interface IComplaintRepository
    {
        Task<Complaint> AddComplaintAsync(Complaint complaint);
        Task<Complaint> UpdateComplaintAsync(Complaint complaint);
        Task<bool> DeleteComplaintAsync(Complaint complaint);
        Task<Complaint?> GetComplaintByComplaintIdAsync(int complaintId);
        Task<List<Complaint>> GetAllComplaintsAsync();
        Task<List<Complaint>> GetRecentComplaintsInConstituencyAsync(int constituencyId);
        Task<List<Complaint>> GetAllReferredComplaintsAsync(int complaintId);
        Task<List<Complaint>> GetComplaintByUserIdAsync(int userId);
        Task<List<Complaint>> GetComplaintsForLeaderAsync(int leaderId);
        Task<List<Complaint>> GetComplaintsForConstituencyAsync(int constituencyId);
        Task<List<Complaint>> GetComplaintsForConstituencyAndStatusAsync(int status, int constituencyId);
        Task<List<Complaint>> GetComplaintsInBackLogForMoreThanHoursAsync(int hours);
        Task<List<Complaint>> GetUnresolvedComplaintsForLeaderAsync(int leaderId);
    }
}
