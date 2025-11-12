namespace WelfareWorkTrackerAuth.Core.Contracts.Repository;
public interface IConstituencyRepository
{
    Task<int> GetConstituencyIdByNameAsync(string name);
}