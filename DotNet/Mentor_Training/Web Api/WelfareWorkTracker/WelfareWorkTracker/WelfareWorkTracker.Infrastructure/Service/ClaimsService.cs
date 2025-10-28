using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using WelfareWorkTracker.Core.Contracts.Repository;
using WelfareWorkTracker.Core.Contracts.Service;

namespace WelfareWorkTracker.Infrastructure.Service
{
    public class ClaimsService(IHttpContextAccessor httpContext, IUserRepository userRepository) : IClaimsService
    {
        private readonly IHttpContextAccessor _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        private ClaimsIdentity GetClaimsIdentity()
        {
            var user = _httpContext.HttpContext?.User
                ?? throw new InvalidOperationException("HttpContext or User is null");

            if (user.Identity is ClaimsIdentity identity)
                return identity;

            throw new InvalidOperationException("User identity is not a ClaimsIdentity.");
        }

        public string GetRoleNameFromClaims()
        {
            var identity = GetClaimsIdentity();
            var roleClaim = identity.FindFirst("Role")
                ?? throw new InvalidOperationException("Role claim not found.");

            return roleClaim.Value;
        }

        public async Task<string> GetUserConstituencyFromClaimsAsync()
        {
            var identity = GetClaimsIdentity();
            var emailClaim = identity.FindFirst("Email")
                ?? throw new InvalidOperationException("Email claim not found.");

            var email = emailClaim.Value;
            var userDetails = await _userRepository.GetUserByEmailAsync(email)
                ?? throw new InvalidOperationException("User not found.");

            return userDetails.ConstituencyName!;
        }

        public async Task<int> GetUserIdFromClaimsAsync()
        {
            var identity = GetClaimsIdentity();
            var emailClaim = identity.FindFirst("Email")
                ?? throw new InvalidOperationException("Email claim not found.");

            var email = emailClaim.Value;
            var userDetails = await _userRepository.GetUserByEmailAsync(email)
                ?? throw new InvalidOperationException("User not found.");

            return userDetails.UserId;
        }
    }
}
