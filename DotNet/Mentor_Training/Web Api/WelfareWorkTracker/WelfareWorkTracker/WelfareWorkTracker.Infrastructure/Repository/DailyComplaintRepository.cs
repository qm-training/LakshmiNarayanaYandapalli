namespace WelfareWorkTracker.Infrastructure.Repository;
public class DailyComplaintRepository(WelfareWorkTrackerContext context) : IDailyComplaintRepository
{
    private readonly WelfareWorkTrackerContext _context = context;

    public async Task<DailyComplaint> AddDailyComplaintAsync(DailyComplaint dailyComplaint)
    {
        await _context.AddAsync(dailyComplaint);
        await _context.SaveChangesAsync();
        return dailyComplaint;
    }

    public async Task<List<DailyComplaint>> GetDailyComplaintsAsync()
    {
        var dailyComplaints = await _context.DailyComplaints.ToListAsync();
        return dailyComplaints;
    }

    public async Task<DailyComplaint?> GetDailyComplaintByIdAsync(int id)
    {
        var dailyComplaint = await _context.DailyComplaints.FindAsync(id);
        return dailyComplaint;
    }

    public async Task<DailyComplaint> UpdateDailyComplaintAsync(DailyComplaint dailyComplaint)
    {
        _context.Update(dailyComplaint);
        await _context.SaveChangesAsync();
        return dailyComplaint;
    }

    public async Task<DailyComplaint?> GetDailyComplaintByLeaderIdAsync(int leaderId)
    {
        var dailyComplaint = await _context.DailyComplaints.FirstOrDefaultAsync(d => d.LeaderId == leaderId && d.DateCreated.Date == DateTime.Today);
        return dailyComplaint;
    }

    public async Task<List<DailyComplaint>> GetTodaysDailyComplaintsAsync()
    {
        var previousDay = DateTime.UtcNow.Date.AddDays(-1);
        var dailyComplaints = await _context.DailyComplaints.Where(t => t.DateCreated.Date == previousDay).ToListAsync();
        return dailyComplaints;
    }
    public async Task<List<DailyComplaint>> GetDailyComplaintsForLeaderByLeaderIdAsync(int leaderId)
    {
        var complaints = await _context.DailyComplaints.Where(l => l.LeaderId == leaderId).ToListAsync();
        return complaints;
    }

    public async Task<DailyComplaint?> GetDailyComplaintsByConstituencyNameAsync(string constituencyName)
    {
        var dailyComplaint = await (from d in _context.DailyComplaints
                                    join c in _context.Constituencies on d.ConstituencyId equals c.ConstituencyId
                                    where c.ConstituencyName == constituencyName && d.DateCreated.Date == DateTime.Today
                                    select d).FirstOrDefaultAsync();

        return dailyComplaint;
    }
}
