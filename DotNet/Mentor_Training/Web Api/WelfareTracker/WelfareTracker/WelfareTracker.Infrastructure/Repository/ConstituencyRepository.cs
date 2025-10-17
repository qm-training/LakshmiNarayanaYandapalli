using Microsoft.EntityFrameworkCore;
using WelfareTracker.Core.Contracts.Repository;
using WelfareTracker.Core.Models;
using WelfareTracker.Infrastructure.Data;

namespace WelfareTracker.Infrastructure.Repository
{
    public class ConstituencyRepository(WelfareTrackerContext context) : IConstituencyRepository
    {
        private readonly WelfareTrackerContext _context = context;
        public async Task<Constituency> GetConstituencyByNameAsync(string constituencyName)
        {
            var constituency = await _context.Constituencies
                .FirstOrDefaultAsync(c => c.ConstituencyName == constituencyName);
            return constituency!;
        }
    }
}
