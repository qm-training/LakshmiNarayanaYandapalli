namespace WelfareWorkTracker.Core.Contracts.Service;
public interface IComplaintService
{
    Task<ComplaintDto> AddComplaintAsync(ComplaintVm complaintVm);
    Task<List<ComplaintDto>> GetComplaintsAsync();
    Task<List<ComplaintDto>> GetComplaintsByUserIdAsync(int userId);
    Task<List<ComplaintDto>> GetRecentComplaintsAsync(string constituencyName);
    Task<ComplaintDto?> GetComplaintByComplaintIdAsync(int complaintId);
    Task<ComplaintDto?> UpdateComplaintByComplaintIdAsync(int complaintId, ComplaintVm complaintVm);
    Task<bool> DeleteComplaintByComplaintIdAsync(int complaintId);
    Task<List<ComplaintDto>?> GetComplaintsForLeaderByLeaderIdAsync(int leaderId, Status status);
    Task CheckComplaintsForLeaderApprovalAsync();
    Task<List<ComplaintDto>> GetComplaintsForAdminRepAsync(string constituencyName);
}