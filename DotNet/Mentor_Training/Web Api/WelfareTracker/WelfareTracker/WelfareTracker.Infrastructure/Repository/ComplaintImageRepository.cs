using Microsoft.EntityFrameworkCore;
using WelfareTracker.Core.Contracts.Repository;
using WelfareTracker.Core.Models;
using WelfareTracker.Infrastructure.Data;

namespace WelfareTracker.Infrastructure.Repository
{
    public class ComplaintImageRepository(WelfareTrackerContext context) : IComplaintImageRepository
    {
        private readonly WelfareTrackerContext _context = context;
        public async Task<ComplaintImage> AddComplaintImageAsync(ComplaintImage complaintImage)
        {
            await _context.ComplaintImages.AddAsync(complaintImage);
            await _context.SaveChangesAsync();
            return complaintImage;
        }

        public async Task<ComplaintImage> DeleteComplaintImageAsync(ComplaintImage complaintImage)
        {
            _context.ComplaintImages.Remove(complaintImage);
            await _context.SaveChangesAsync();
            return complaintImage;

        }

        public async Task<List<ComplaintImage>> GetComplaintImagesByComplaintIdAsync(int complaintId)
        {
            var complaintImages = await _context.ComplaintImages.Where(ci => ci.ComplaintId == complaintId).ToListAsync();
            return complaintImages;
        }
    }
}
