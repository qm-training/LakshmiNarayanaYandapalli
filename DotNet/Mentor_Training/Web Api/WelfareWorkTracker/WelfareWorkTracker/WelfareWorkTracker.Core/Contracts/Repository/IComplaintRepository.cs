namespace WelfareWorkTracker.Core.Contracts.Repository;
public interface IComplaintRepository
{
    Task<Complaint> AddComplaintAsync(Complaint complaint);
    Task<List<Complaint>> GetComplaintsAsync();
    Task<List<Complaint>> GetComplaintsByUserIdAsync(int userId);
    Task<List<Complaint>> GetRecentComplaintsAsync(string constituencyName);
    Task<Complaint?> GetComplaintByComplaintIdAsync(int complaintId);
    Task<List<Complaint>> GetComplaintsByComplaintIdsAsync(List<int> complaintIds);
    Task UpdateComplaintByIdAsync(Complaint complaint);
    Task DeleteComplaintByComplaintAsync(Complaint complaint);
    Task<List<Complaint>> GetComplaintsByConstituency(string constituencyName);
    Task<List<Complaint>> GetComplaintsForLeaderByLeaderIdAsync(int leaderId);
    Task<List<ComplaintStatus?>> GetComplaintsInBacklogForMoreThanHoursAsync(int hours);
}