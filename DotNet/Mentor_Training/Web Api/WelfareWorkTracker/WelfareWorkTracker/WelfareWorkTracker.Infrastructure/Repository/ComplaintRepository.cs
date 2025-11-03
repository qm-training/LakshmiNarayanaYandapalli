using Microsoft.EntityFrameworkCore;
using WelfareWorkTracker.Core.Contracts.Repository;
using WelfareWorkTracker.Core.Enums;
using WelfareWorkTracker.Core.Models;
using WelfareWorkTracker.Infrastructure.Data;

namespace WelfareWorkTracker.Infrastructure.Repository
{
    public class ComplaintRepository(WelfareWorkTrackerContext context) : IComplaintRepository
    {
        private readonly WelfareWorkTrackerContext _context = context;
        public async Task<Complaint> AddComplaintAsync(Complaint complaint)
        {
            await _context.Complaints.AddAsync(complaint);
            await _context.SaveChangesAsync();
            return complaint;
        }

        public async Task<bool> DeleteComplaintAsync(Complaint complaint)
        {
            _context.Complaints.Remove(complaint);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Complaint>> GetAllComplaintsAsync()
        {
            var complaints = await _context.Complaints.ToListAsync();
            return complaints;
        }

        public async Task<List<Complaint>> GetAllReferredComplaintsAsync(int complaintId)
        {
            var complaints = await _context.Complaints.Where(c => c.ReferenceNumber == complaintId).ToListAsync();
            return complaints;
        }

        public async Task<Complaint?> GetComplaintByComplaintIdAsync(int complaintId)
        {
            var complaint = await _context.Complaints.FindAsync(complaintId);
            return complaint;
        }

        public async Task<List<Complaint>> GetComplaintByUserIdAsync(int userId)
        {
            var complaints = await _context.Complaints.Where(c => c.CitizenId == userId).ToListAsync();
            return complaints;
        }

        public async Task<List<Complaint>> GetComplaintsForConstituencyAndStatusAsync(int status, int constituencyId)
        {
            var complaints = await _context.Complaints.Where(c => c.Status == status && c.ConstituencyId == constituencyId).ToListAsync();
            return complaints;
        }

        public async Task<List<Complaint>> GetComplaintsForConstituencyAsync(int constituencyId)
        {
            var complaints = await _context.Complaints.Where(c => c.ConstituencyId == constituencyId).ToListAsync();
            return complaints;
        }

        public async Task<List<Complaint>> GetComplaintsForLeaderAsync(int leaderId)
        {
            var complaints = await _context.Complaints.Where(c => c.LeaderId == leaderId).ToListAsync();
            return complaints;
        }

        public Task<List<Complaint>> GetComplaintsInBackLogForMoreThanHoursAsync(int hours)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Complaint>> GetRecentComplaintsInConstituencyAsync(int constituencyId)
        {
            var complaints = await _context.Complaints
                            .Where(c => c.ConstituencyId == constituencyId)
                            .OrderByDescending(c => c.DateCreated)
                            .Take(10)
                            .ToListAsync();

            return complaints;
        }

        public async Task<List<Complaint>> GetUnresolvedComplaintsForLeaderAsync(int leaderId)
        {
            var unresolvedComplaints = await _context.Complaints.Where(c => c.Status == (int)Status.Unresolved).ToListAsync();
            return unresolvedComplaints;
        }

        public async Task<Complaint> UpdateComplaintAsync(Complaint complaint)
        {
            _context.Complaints.Update(complaint);
            await _context.SaveChangesAsync();
            return complaint;
        }
    }
}
