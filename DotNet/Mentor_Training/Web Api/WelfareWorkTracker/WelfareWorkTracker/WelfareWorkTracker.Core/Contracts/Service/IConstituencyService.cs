namespace WelfareWorkTracker.Core.Contracts.Service;
public interface IConstituencyService
{
    Task<List<ConstituencyDto>> GetAllConstituenciesAsync();
    Task<ConstituencyDto> AddConstituencyAsync(ConstituencyVm constituencyVm);
}