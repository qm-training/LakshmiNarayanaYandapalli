using WelfareTracker.Core.Models;

namespace WelfareTracker.Core.Contracts.Repository
{
    public interface IComplaintImageRepository
    {
        Task<ComplaintImage> AddComplaintImageAsync(ComplaintImage complaintImage);
        Task<ComplaintImage> DeleteComplaintImageAsync(ComplaintImage complaintImage);
        Task<List<ComplaintImage>> GetComplaintImagesByComplaintIdAsync(int complaintId);
    }
}
