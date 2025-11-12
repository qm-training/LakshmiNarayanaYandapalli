namespace WelfareWorkTracker.Infrastructure.Repository;
public class ComplaintRepository(WelfareWorkTrackerContext context) : IComplaintRepository
{
    private readonly WelfareWorkTrackerContext _context = context;
    public async Task<Complaint> AddComplaintAsync(Complaint complaint)
    {
        await _context.AddAsync(complaint);
        await _context.SaveChangesAsync();
        return complaint;
    }

    public async Task DeleteComplaintByComplaintAsync(Complaint complaint)
    {
        _context.Complaints.Remove(complaint);
        await _context.SaveChangesAsync();
    }

    public async Task<Complaint?> GetComplaintByComplaintIdAsync(int complaintId)
    {
        var complaint = await _context.Complaints.FirstOrDefaultAsync(c => c.ComplaintId == complaintId);
        return complaint;
    }

    public async Task<List<Complaint>> GetComplaintsAsync()
    {
        var complaints = await _context.Complaints.ToListAsync();
        return complaints;
    }

    public async Task<List<Complaint>> GetComplaintsByComplaintIdsAsync(List<int> complaintIds)
    {
        return await _context.Complaints.Where(c => complaintIds.Contains(c.ComplaintId)).ToListAsync();
    }

    public async Task<List<Complaint>> GetComplaintsByConstituency(string constituencyName)
    {
        var complaints = await _context.Complaints.Where(c => c.ConstituencyName == constituencyName).ToListAsync();
        return complaints;
    }

    public async Task<List<Complaint>> GetComplaintsByUserIdAsync(int userId)
    {
        var complaints = await _context.Complaints.Where(c => c.CitizenId == userId).ToListAsync();
        return complaints;
    }

    public async Task<List<Complaint>> GetComplaintsForLeaderByLeaderIdAsync(int leaderId)
    {
        var complaints = await _context.Complaints.Where(c => c.LeaderId == leaderId).ToListAsync();
        return complaints;
    }

    public async Task<List<ComplaintStatus?>> GetComplaintsInBacklogForMoreThanHoursAsync(int hours)
    {
        var cutoffDate = DateTime.Now.AddHours(-hours);

        // Fetching the latest status for each complaint
        var latestComplaints = await _context.ComplaintStatuses
            .GroupBy(c => c.ComplaintId)
            .Select(g => g.OrderByDescending(c => c.DateUpdated).FirstOrDefault())
            .ToListAsync();

        // Filtering complaints that are in backlog and older than the cutoff date
        var complaintsInBacklog = latestComplaints
            .Where(c => c!.Status == (int)Status.Backlog && c.OpenedDate <= cutoffDate)
            .ToList();

        return complaintsInBacklog;
    }

    public async Task<List<Complaint>> GetRecentComplaintsAsync(string constituencyName)
    {
        var complaints = await _context.Complaints
                        .Where(c => c.ConstituencyName == constituencyName)
                        .OrderByDescending(c => c.DateCreated)
                        .Take(10)
                        .ToListAsync();

        return complaints;
    }

    public async Task UpdateComplaintByIdAsync(Complaint complaint)
    {
        _context.Complaints.Update(complaint);
        await _context.SaveChangesAsync();
    }
}
