namespace WelfareWorkTracker.Core.Contracts.Service;
public interface IComplaintImageService
{
    Task<ComplaintImageDto?> AddComplaintImageAsync(ComplaintImageVm complaintImageVm);
    Task<ComplaintImageDto?> GetComplaintImageByIdAsync(int id);
    Task<bool> DeleteComplaintImageByIdAsync(int complaintImageId);
    Task<List<ComplaintImageDto>?> GetComplaintImagesByComplaintIdAsync(int complaintId);
}