using Microsoft.EntityFrameworkCore;
using WelfareWorkTracker.Core.Contracts.Repository;
using WelfareWorkTracker.Core.Models;
using WelfareWorkTracker.Infrastructure.Data;

namespace WelfareWorkTracker.Infrastructure.Repository
{
    public class DailyComplaintRepository(WelfareWorkTrackerContext context) : IDailyComplaintRepository
    {
        private readonly WelfareWorkTrackerContext _context = context;
        public async Task<DailyComplaint> AddDailyComplaintAsync(DailyComplaint dailyComplaint)
        {
            await _context.AddAsync(dailyComplaint);
            await _context.SaveChangesAsync();
            return dailyComplaint;
        }

        public async Task<List<DailyComplaint>> GetAllDailyComplaintsAsync()
        {
            var dailyComplaints = await _context.DailyComplaints.ToListAsync();
            return dailyComplaints;
        }

        public async Task<List<DailyComplaint>> GetAllDailyComplaintsForLeader(int leaderId)
        {
            var dailyComplaints = await _context.DailyComplaints.Where(d => d.LeaderId == leaderId).ToListAsync();
            return dailyComplaints;
        }

        public Task<List<DailyComplaint>> GetAllDailyComplaintsForLeaderAsync(int leaderId)
        {
            throw new NotImplementedException();
        }

        public async Task<DailyComplaint?> GetDailyComplaintByIdAsync(int dailyComplaintId)
        {
            var dailyComplaint = await _context.DailyComplaints.FindAsync(dailyComplaintId);
            return dailyComplaint;
        }

        public async Task<DailyComplaint?> GetDailyComplaintForLeaderByLeaderIdAsync(int leaderId)
        {
            var dailyComplaint = await _context.DailyComplaints.Where(d => d.LeaderId == leaderId).FirstOrDefaultAsync();
            return dailyComplaint;
        }

        public async Task<List<DailyComplaint>> GetTodaysDailyComplaintsAsync()
        {
            var previousDay = DateTime.UtcNow.Date.AddDays(-1);
            var dailyComplaints = await _context.DailyComplaints.Where(t => t.DateCreated.Date == previousDay).ToListAsync();
            return dailyComplaints;
        }

        public async Task<DailyComplaint> UpdateDailyComplaintAsync(DailyComplaint dailyComplaint)
        {
            _context.DailyComplaints.Update(dailyComplaint);
            await _context.SaveChangesAsync();
            return dailyComplaint;
        }
    }
}
