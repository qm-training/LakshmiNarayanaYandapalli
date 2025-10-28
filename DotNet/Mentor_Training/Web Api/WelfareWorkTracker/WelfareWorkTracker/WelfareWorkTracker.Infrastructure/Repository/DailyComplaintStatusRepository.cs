using Microsoft.EntityFrameworkCore;
using WelfareWorkTracker.Core.Contracts.Repository;
using WelfareWorkTracker.Core.Enums;
using WelfareWorkTracker.Core.Models;
using WelfareWorkTracker.Infrastructure.Data;

namespace WelfareWorkTracker.Infrastructure.Repository
{
    public class DailyComplaintStatusRepository(WelfareWorkTrackerContext context,
                                                    IDailyComplaintRepository dailyComplaintRepository) : IDailyComplaintStatusRepository
    {
        private readonly WelfareWorkTrackerContext _context = context;
        private readonly IDailyComplaintRepository _dailyComplaintRepository = dailyComplaintRepository;
        public async Task<DailyComplaintStatus> AddDailyComplaintStatusAsync(DailyComplaintStatus dailyComplaintStatus)
        {
            await _context.DailyComplaintStatuses.AddAsync(dailyComplaintStatus);
            await _context.SaveChangesAsync();
            return dailyComplaintStatus;
        }

        public async Task<DailyComplaintStatus?> GetDailyComplaintStatusByDailyComplaintIdAsync(int dailyComplaintId)
        {
            var dailyComplaintStatus = await _context.DailyComplaintStatuses
                .Where(cs => cs.DailyComplaintId == dailyComplaintId)
                .OrderByDescending(cs => cs.DateUpdated)
                .FirstOrDefaultAsync();

            return dailyComplaintStatus;
        }

        public async Task<DailyComplaintStatus?> GetDailyComplaintStatusByDailyComplaintStatusIdAsync(int dailyComplaintStatusId)
        {
            var dailyComplaintStatus = await _context.DailyComplaintStatuses.FindAsync(dailyComplaintStatusId);
            return dailyComplaintStatus;
        }

        public async Task<List<DailyComplaintStatus?>> GetPendingDailyComplaintsAsync()
        {
            var latestComplaints = await _context.DailyComplaintStatuses
                                        .GroupBy(d => d.DailyComplaintId)
                                        .Select(g => g.OrderByDescending(cs => cs.DateUpdated).FirstOrDefault())
                                        .ToListAsync();

            var pendingComplaints = latestComplaints.Where(l => l!.Status != 8).ToList();
            return pendingComplaints;
        }

        public async Task<List<DailyComplaintStatus>> GetUnresolvedComplaintsOfLeader(int leaderId, int status)
        {
            var complaints = await _dailyComplaintRepository.GetAllDailyComplaintsForLeaderAsync(leaderId);
            var unresolvedComplaints = await _context.DailyComplaintStatuses
                                                    .Where(cs => complaints.Select(c => c.DailyComplaintId)
                                                    .Contains(cs.DailyComplaintId) && cs.Status == status)
                                                    .ToListAsync();

            return unresolvedComplaints;
        }
    }
}
