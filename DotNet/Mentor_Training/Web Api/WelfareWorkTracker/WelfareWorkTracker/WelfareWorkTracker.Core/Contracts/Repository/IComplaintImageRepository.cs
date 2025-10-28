using WelfareWorkTracker.Core.Models;

namespace WelfareWorkTracker.Core.Contracts.Repository
{
    public interface IComplaintImageRepository
    {
        Task<ComplaintImage> AddComplaintImageAsync(ComplaintImage complaintImage);
        Task<ComplaintImage?> GetComplaintImageByIdAsync(int id);
        Task<List<ComplaintImage>> GetAllComplaintImagesByComplaintIdAsync(int complaintId);
        Task<bool> RemoveComplaintImageAsync(ComplaintImage complaintImage);
    }
}
