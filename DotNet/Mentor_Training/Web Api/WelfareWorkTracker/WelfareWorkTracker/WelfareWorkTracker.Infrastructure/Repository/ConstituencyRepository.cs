namespace WelfareWorkTracker.Infrastructure.Repository;
public class ConstituencyRepository(WelfareWorkTrackerContext context) : IConstituencyRepository
{
    private readonly WelfareWorkTrackerContext _context = context;

    public async Task<Constituency> AddConstituencyAsync(Constituency constituency)
    {
        await _context.Constituencies.AddAsync(constituency);
        await _context.SaveChangesAsync();
        return constituency;
    }

    public async Task<List<Constituency>> GetConstituenciesAsync()
    {
        var constituencies = await _context.Constituencies.ToListAsync();
        return constituencies;
    }

    public async Task<int> GetConstituencyIdByNameAsync(string name)
    {
        var constituency = await _context.Constituencies
                                        .Where(c => c.ConstituencyName == name)
                                        .FirstOrDefaultAsync();

        return constituency == null ? 0 : constituency.ConstituencyId;
    }

    public async Task<string?> GetConstituencyNameByIdAsync(int id)
    {
        var constituency = await _context.Constituencies.FindAsync(id);
        return constituency?.ConstituencyName;
    }
}