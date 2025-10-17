using WelfareTracker.Core.Models;

namespace WelfareTracker.Core.Contracts.Repository
{
    public interface IConstituencyRepository
    {
        Task<Constituency> GetConstituencyByNameAsync(string constituencyName);
    }
}
