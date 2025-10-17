using Microsoft.EntityFrameworkCore;
using WelfareTracker.Core.Contracts.Repository;
using WelfareTracker.Core.Models;
using WelfareTracker.Infrastructure.Data;

namespace WelfareTracker.Infrastructure.Repository
{
    public class ComplaintRepository(WelfareTrackerContext context) : IComplaintRepository
    {
        private readonly WelfareTrackerContext _context = context;
        public async Task<Complaint> AddComplaintAsync(Complaint complaint)
        {
            await _context.Complaints.AddAsync(complaint);
            await _context.SaveChangesAsync();
            return complaint;
        }

        public async Task<Complaint?> GetComplaintByIdAsync(int complaintId)
        {
            var complaint = await _context.Complaints.FindAsync(complaintId);
            return complaint;
        }

        public async Task<List<Complaint>> GetComplaintsByCitizenIdAsync(int citizenId)
        {
            var complaints = await _context.Complaints.Where(c => c.CitizenId == citizenId).ToListAsync();
            return complaints;
        }

        public async Task<Complaint> UpdateComplaintAsync(Complaint complaint)
        {
            _context.Complaints.Update(complaint);
            await _context.SaveChangesAsync();
            return complaint;
        }

    }
}
