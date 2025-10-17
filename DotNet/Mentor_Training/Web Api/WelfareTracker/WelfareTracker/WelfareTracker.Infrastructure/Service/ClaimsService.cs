using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using WelfareTracker.Core.Contracts.Repository;
using WelfareTracker.Core.Contracts.Service;

namespace WelfareTracker.Infrastructure.Service;
public class ClaimsService(IHttpContextAccessor httpContext, IUserRepository userRepository) : IClaimsService
{
    private readonly IHttpContextAccessor _httpContext = httpContext;
    private readonly IUserRepository _userRepository = userRepository;

    public ClaimsIdentity GetClaimsIdentity()
    {
        var claimsIdentity = _httpContext.HttpContext?.User.Identity as ClaimsIdentity;
        if (claimsIdentity == null)
        {
            throw new InvalidOperationException("Claims identity not found in HTTP context.");
        }
        return claimsIdentity;
    }

    public async Task<string> GetConstituencyNameFromClaimsAsync()
    {
        var identity = GetClaimsIdentity();
        var emailClaim = identity.FindFirst(ClaimTypes.Email) ??
            throw new InvalidOperationException("Email claim not found.");

        var user = await _userRepository.GetUserByEmailAsync(emailClaim.Value) ??
            throw new InvalidOperationException("User not found.");

        return user.ConstituencyName!;
    }

    public string GetRoleNameFromClaimsAsync()
    {
        var identity = GetClaimsIdentity();

        var roleClaim = identity.FindFirst(ClaimTypes.Role) ?? 
            throw new InvalidOperationException("Role claim not found.");

        return roleClaim.Value;
    }

    public async Task<int> GetUserIdFromClaimsAsync()
    {
        var identity = GetClaimsIdentity();
        var emailClaim = identity.FindFirst(ClaimTypes.Email) ??
            throw new InvalidOperationException("Email claim not found.");

        var user = await _userRepository.GetUserByEmailAsync(emailClaim.Value) ??
            throw new InvalidOperationException("User not found.");

        return user.UserId;
    }
}
