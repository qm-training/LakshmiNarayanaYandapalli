namespace WelfareTracker.Core.Contracts.Service;
public interface IClaimsService
{
    Task<int> GetUserIdFromClaimsAsync();
    Task<string> GetConstituencyNameFromClaimsAsync();
    string GetRoleNameFromClaimsAsync();

}
