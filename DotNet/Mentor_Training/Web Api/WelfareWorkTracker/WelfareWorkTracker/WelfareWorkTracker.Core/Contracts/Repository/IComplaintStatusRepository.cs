namespace WelfareWorkTracker.Core.Contracts.Repository;
public interface IComplaintStatusRepository
{
    Task<ComplaintStatus> AddComplaintStatusAsync(ComplaintStatus complaintStatus);
    Task<ComplaintStatus> UpdateComplaintStatus(ComplaintStatus complaintStatus);
    Task<ComplaintStatus?> GetComplaintStatusAsync(int? complaintId = null, int? complaintStatusId = null);
    Task<bool> DeleteComplaintStatusByComplaintIdAsync(int complaintId);
    Task<List<ComplaintStatus>> GetUnresolvedComplaintsOfLeader(int leaderId, Status status);
    Task<List<ComplaintStatus>> GetComplaintStatusHistoryByComplaintIdAsync(int complaintId);
}
