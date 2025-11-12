namespace WelfareWorkTracker.Core.Extensions;
public static class ClaimsExtensions
{
    public static int GetUserIdAsInt(this ClaimsPrincipal user)
    {
        var userIdString = user.FindFirst(CustomClaimTypes.UserId)?.Value;
        if (string.IsNullOrEmpty(userIdString))
            throw new WelfareWorkTrackerException("Unable to Find Id in Claims!");

        var isNumber = int.TryParse(userIdString, out int userId);
        return isNumber ? userId : 0;
    }

    public static string GetUserEmail(this ClaimsPrincipal user)
    {
        var userEmail = user.FindFirst(CustomClaimTypes.UserEmail)?.Value;
        if (string.IsNullOrEmpty(userEmail))
            throw new WelfareWorkTrackerException("Unable to Find Email in Claims!");

        return userEmail;
    }
}
