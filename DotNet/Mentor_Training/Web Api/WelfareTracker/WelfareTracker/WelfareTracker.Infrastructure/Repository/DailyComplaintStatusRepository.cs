using Microsoft.EntityFrameworkCore;
using WelfareTracker.Core.Contracts.Repository;
using WelfareTracker.Core.Models;
using WelfareTracker.Infrastructure.Data;

namespace WelfareTracker.Infrastructure.Repository
{
    public class DailyComplaintStatusRepository(WelfareTrackerContext context) : IDailyComplaintStatusRepository
    {
        private readonly WelfareTrackerContext _context = context;
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
    }
}
