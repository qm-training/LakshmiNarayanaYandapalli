using Microsoft.EntityFrameworkCore;
using WelfareTracker.Core.Contracts.Repository;
using WelfareTracker.Core.Models;
using WelfareTracker.Infrastructure.Data;

namespace WelfareTracker.Infrastructure.Repository
{
    public class ComplaintStatusRepository(WelfareTrackerContext context) : IComplaintStatusRepository
    {
        private readonly WelfareTrackerContext _context = context;
        public async Task<ComplaintStatus> AddComplaintStatusAsync (ComplaintStatus complaintStatus)
        {
            await _context.ComplaintStatuses.AddAsync(complaintStatus);
            await _context.SaveChangesAsync();
            return complaintStatus;
        }

        public async Task<ComplaintStatus?> GetComplaintStatusByIdAsync(int complaintStatusId)
        {
            var complaintStatus = await _context.ComplaintStatuses
                                    .Where(cs => cs.ComplaintStatusId == complaintStatusId)
                                    .OrderByDescending(cs => cs.DateCreated)
                                .FirstOrDefaultAsync();

            return complaintStatus;
        }

        public async Task<bool> DeleteComplaintStatusByComplaintIdAsync(int complaintId)
        {
            var complaintStatuses = await _context.ComplaintStatuses.Where(cs => cs.ComplaintId == complaintId).ToListAsync();

            if (complaintStatuses.Count == 0)
            {
                return false;
            }
            _context.ComplaintStatuses.RemoveRange(complaintStatuses);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ComplaintStatus?> GetComplaintStatusByComplaintIdAsync(int complaintId)
        {
            var complaintStatus = await _context.ComplaintStatuses
                                    .Where(cs => cs.ComplaintId == complaintId)
                                    .OrderByDescending(cs => cs.DateCreated)
                                .FirstOrDefaultAsync();
            return complaintStatus;
        }
    }
}
