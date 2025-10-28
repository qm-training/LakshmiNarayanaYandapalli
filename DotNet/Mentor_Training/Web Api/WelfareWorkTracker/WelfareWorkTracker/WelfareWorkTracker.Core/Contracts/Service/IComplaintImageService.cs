using WelfareWorkTracker.Core.Dtos;
using WelfareWorkTracker.Core.Vms;

namespace WelfareWorkTracker.Core.Contracts.Service
{
    public interface IComplaintImageService
    {
        Task<ComplaintImageDto> AddComplaintImageAsync(ComplaintImageVm complaintImageVm);
        Task<bool> DeleteComplaintImageByIdAsync(int complaintImageId);
        Task<ComplaintImageDto> GetComplaintImageByIdAsync(int complaintImageId);
        Task<List<ComplaintImageDto>> GetComplaintImagesByComplaintIdAsync(int complaintId);
    }
}
