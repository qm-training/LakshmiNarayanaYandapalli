namespace WelfareWorkTracker.Core.Contracts.Repository;
public interface IConstituencyRepository
{
    Task<Constituency> AddConstituencyAsync(Constituency constituency);
    Task<string?> GetConstituencyNameByIdAsync(int id);
    Task<int> GetConstituencyIdByNameAsync(string name);
    Task<List<Constituency>> GetConstituenciesAsync();
}