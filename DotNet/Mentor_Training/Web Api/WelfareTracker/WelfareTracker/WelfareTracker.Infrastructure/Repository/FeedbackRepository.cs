using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WelfareTracker.Core.Contracts.Repository;
using WelfareTracker.Core.Exceptions;
using WelfareTracker.Core.Models;
using WelfareTracker.Infrastructure.Data;

namespace WelfareTracker.Infrastructure.Repository
{
    public class FeedbackRepository(WelfareTrackerContext context) : IFeedbackRepository
    {
        private readonly WelfareTrackerContext _context = context;

        public async Task<ComplaintFeedback> AddFeedbackAsync(ComplaintFeedback citizenFeedback)
        {
            await _context.ComplaintFeedbacks.AddAsync(citizenFeedback);
            await _context.SaveChangesAsync();
            return citizenFeedback;
        }

        public async Task<List<ComplaintFeedback>> GetAllFeedbacksAsync(int? complaintId, int? dailyComplaintId)
        {
            if (complaintId.HasValue)
            {
                var feedback = await _context.ComplaintFeedbacks.Where(c => c.ComplaintId == complaintId).ToListAsync();
                return feedback;
            }
            else if (dailyComplaintId.HasValue)
            {
                var feedback = await _context.ComplaintFeedbacks.Where(d => d.DailyComplaintId == dailyComplaintId).ToListAsync();
                return feedback;
            }
            throw new WelfareTrackerException();
        }

        public async Task<ComplaintFeedback?> GetFeedbackByUserAsync(int userId, int? complaintId, int? dailyComplaintId)
        {
            if (complaintId.HasValue)
            {
                var complaintFeedback = await _context.ComplaintFeedbacks.FirstOrDefaultAsync(c => c.ComplaintId == complaintId && c.CitizenId == userId);
                return complaintFeedback;
            }
            else if (dailyComplaintId.HasValue)
            {
                var dailyComplaintFeedback = await _context.ComplaintFeedbacks.FirstOrDefaultAsync(d => d.DailyComplaintId == dailyComplaintId && d.CitizenId == userId);
                return dailyComplaintFeedback;
            }
            throw new WelfareTrackerException();
        }

        public async Task<int> GetSatisfiedCount(int? complaintId = null, int? dailyComplaintId = null)
        {
            if (complaintId.HasValue)
            {
                return await _context.ComplaintFeedbacks
                    .CountAsync(f => f.ComplaintId == complaintId.Value && f.IsSatisfied);
            }
            else if (dailyComplaintId.HasValue)
            {
                return await _context.ComplaintFeedbacks
                    .CountAsync(f => f.DailyComplaintId == dailyComplaintId.Value && f.IsSatisfied);
            }
            return 0;
        }

        public async Task<int> GetUnSatisfiedCount(int? complaintId = null, int? dailyComplaintId = null)
        {
            if (complaintId.HasValue)
            {
                return await _context.ComplaintFeedbacks
                    .CountAsync(f => f.ComplaintId == complaintId.Value && !f.IsSatisfied);
            }
            else if (dailyComplaintId.HasValue)
            {
                return await _context.ComplaintFeedbacks
                    .CountAsync(f => f.DailyComplaintId == dailyComplaintId.Value && !f.IsSatisfied);
            }
            return 0;
        }
    }
}
