using Microsoft.EntityFrameworkCore;
using WelfareWorkTracker.Core.Contracts.Repository;
using WelfareWorkTracker.Core.Models;
using WelfareWorkTracker.Infrastructure.Data;

namespace WelfareWorkTracker.Infrastructure.Repository
{
    public class ComplaintStatusRepository(WelfareWorkTrackerContext context,
                                                IComplaintRepository complaintRepository) : IComplaintStatusRepository
    {
        private readonly WelfareWorkTrackerContext _context = context;
        private readonly IComplaintRepository _complaintRepository = complaintRepository;
        public async Task<ComplaintStatus> AddComplaintStatusAsync(ComplaintStatus complaintStatus)
        {
            await _context.ComplaintStatuses.AddAsync(complaintStatus);
            await _context.SaveChangesAsync();
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

        public async Task<ComplaintStatus?> GetComplaintStatusByIdAsync(int complaintStatusId)
        {
            var complaintStatus = await _context.ComplaintStatuses
                                                .Where(cs => cs.ComplaintStatusId == complaintStatusId)
                                                .OrderByDescending(cs => cs.DateCreated)
                                                .FirstOrDefaultAsync();

            return complaintStatus;
        }

        public async Task<List<ComplaintStatus>> GetComplaintStatusHistoryByComplaintIdAsync(int complaintId)
        {
            var complaintStatusHistory =  await _context.ComplaintStatuses
                                                        .Where(c => c.ComplaintId == complaintId)
                                                        .OrderBy(c => c.DateCreated)
                                                        .ToListAsync();
            return complaintStatusHistory;
        }

        public async Task<List<ComplaintStatus>> GetUnresolvedComplaintsOfLeader(int leaderId, int status)
        {
            var complaints = await _complaintRepository.GetComplaintsForLeaderAsync(leaderId);
            var unresolvedComplaints = await _context.ComplaintStatuses
                                                     .Where(cs => complaints.Select(c => c.ComplaintId)
                                                     .Contains(cs.ComplaintId) && cs.Status == status)
                                                     .ToListAsync();

            return unresolvedComplaints;
        }
    }
}
