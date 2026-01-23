namespace WelfareWorkTrackerAuth.Infrastructure.Repository;
public class ConstituencyRepository(WelfareWorkTrackerContext context) : IConstituencyRepository
{
    private readonly WelfareWorkTrackerContext _context = context;

    public async Task<int> GetConstituencyIdByNameAsync(string name)
    {
        var constituency = await _context.Constituencies.Where(c => c.ConstituencyName == name).FirstOrDefaultAsync();
        return constituency == null ? 0 : constituency.ConstituencyId;
    }
}