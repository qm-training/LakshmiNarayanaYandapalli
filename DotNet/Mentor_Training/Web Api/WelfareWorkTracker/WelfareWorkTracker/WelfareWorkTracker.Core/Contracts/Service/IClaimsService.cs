namespace WelfareWorkTracker.Core.Contracts.Service;
public interface IClaimsService
{
    Task<string> GetUserConstituencyFromClaimsAsync();
    Task<int> GetUserIdFromClaimsAsync();
    string GetRoleNameFromClaims();
}