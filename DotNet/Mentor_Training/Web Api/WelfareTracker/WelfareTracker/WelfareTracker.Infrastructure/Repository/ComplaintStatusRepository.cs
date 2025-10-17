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
            var complaintStatus = await _context.ComplaintStatuses.FindAsync(complaintStatusId);
            return complaintStatus;
        }
    }
}
