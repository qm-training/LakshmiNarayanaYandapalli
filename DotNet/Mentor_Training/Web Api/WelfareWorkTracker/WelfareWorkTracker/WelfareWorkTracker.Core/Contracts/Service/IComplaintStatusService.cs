namespace WelfareWorkTracker.Core.Contracts.Service;
public interface IComplaintStatusService
{
    Task<ComplaintStatusDto?> AddComplaintStatusByAdminRepAsync(StatusByAdminRepVm statusByAdminRepVm);
    Task<ComplaintStatusDto?> AddComplaintStatusByAdminAsync(StatusByAdminVm statusByAdminVm);
    Task<ComplaintStatusDto?> GetComplaintStatusByIdAsync(int complaintStatusId);
    Task<ComplaintStatusDto?> GetComplaintStatusByComplaintId(int complaintId);
    Task<ComplaintStatusDto?> UpdateComplaintStatusByLeaderAsync(StatusByLeaderVm statusUpdateVm);
    Task<List<ComplaintStatusDto>> GetComplaintStatusHistoryAsync(int complaintId);
}