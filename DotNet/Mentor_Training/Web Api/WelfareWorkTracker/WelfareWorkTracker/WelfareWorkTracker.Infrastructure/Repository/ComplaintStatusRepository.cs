namespace WelfareWorkTracker.Infrastructure.Repository;
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

    public async Task<ComplaintStatus?> GetComplaintStatusAsync(int? complaintId = null, int? complaintStatusId = null)
    {
        if (complaintId.HasValue)
        {
            // Fetching the latest status for a given complaintId
            return await _context.ComplaintStatuses
                .Where(cs => cs.ComplaintId == complaintId.Value)
                .OrderByDescending(cs => cs.DateUpdated)
                .FirstOrDefaultAsync();
        }

        if (complaintStatusId.HasValue)
        {
            // Fetching the complaint status by its ID
            return await _context.ComplaintStatuses.FirstOrDefaultAsync(c => c.ComplaintStatusId == complaintStatusId);
        }

        throw new ArgumentException("At least one of complaintId or complaintStatusId must be provided.");
    }

    public async Task<ComplaintStatus> UpdateComplaintStatus(ComplaintStatus complaintStatus)
    {
        _context.ComplaintStatuses.Update(complaintStatus);
        await _context.SaveChangesAsync();
        return complaintStatus;
    }

    public async Task<bool> DeleteComplaintStatusByComplaintIdAsync(int complaintId)
    {
        var status = await _context.ComplaintStatuses
            .FirstOrDefaultAsync(s => s.ComplaintId == complaintId && s.Status == 1);

        if (status == null)
            return false;

        _context.ComplaintStatuses.Remove(status);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ComplaintStatus>> GetUnresolvedComplaintsOfLeader(int leaderId, Status status)
    {
        var complaints = await _complaintRepository.GetComplaintsForLeaderByLeaderIdAsync(leaderId);
        var unresolvedComplaints = await _context.ComplaintStatuses
       .Where(cs => complaints.Select(c => c.ComplaintId).Contains(cs.ComplaintId) && cs.Status == (int)status)
       .ToListAsync();

        return unresolvedComplaints;
    }

    public async Task<List<ComplaintStatus>> GetComplaintStatusHistoryByComplaintIdAsync(int complaintId)
    {
        return await _context.ComplaintStatuses
            .Where(c => c.ComplaintId == complaintId)
            .OrderBy(c => c.DateUpdated)
            .ToListAsync();
    }
}