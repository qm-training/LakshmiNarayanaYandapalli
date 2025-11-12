namespace WelfareWorkTracker.Infrastructure.Repository;
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

    public async Task<DailyComplaintStatus?> GetDailyComplaintStatusAsync(int? dailyComplaintId = null, int? dailyComplaintStatusId = null)
    {
        if (dailyComplaintId.HasValue)
        {
            // Fetching the latest status for a given complaintId
            return await _context.DailyComplaintStatuses
                .Where(cs => cs.DailyComplaintId == dailyComplaintId.Value)
                .OrderByDescending(cs => cs.DateUpdated)
                .FirstOrDefaultAsync();
        }

        if (dailyComplaintStatusId.HasValue)
        {
            // Fetching the complaint status by its ID
            return await _context.DailyComplaintStatuses.FirstOrDefaultAsync(c => c.DailyComplaintStatusId == dailyComplaintStatusId);
        }

        throw new ArgumentException("At least one of complaintId or complaintStatusId must be provided.");
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

    public async Task<List<DailyComplaintStatus>> GetUnresolvedComplaintsOfLeader(int leaderId, Status status)
    {
        var complaints = await _dailyComplaintRepository.GetDailyComplaintsForLeaderByLeaderIdAsync(leaderId);
        var unresolvedComplaints = await _context.DailyComplaintStatuses
    .Where(cs => complaints.Select(c => c.DailyComplaintId).Contains(cs.DailyComplaintId) && cs.Status == (int)status)
    .ToListAsync();

        return unresolvedComplaints;
    }
}
