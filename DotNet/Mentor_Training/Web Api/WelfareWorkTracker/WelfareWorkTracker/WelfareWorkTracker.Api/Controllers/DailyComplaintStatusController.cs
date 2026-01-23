namespace WelfareWorkTracker.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class DailyComplaintStatusController (IDailyComplaintStatusService dailyComplaintStatusService) : ControllerBase
{
    private readonly IDailyComplaintStatusService _dailyComplaintStatusService = dailyComplaintStatusService;

    /// <summary>
    /// Updates the status of a daily complaint by a leader.
    /// </summary>
    /// <param name="dailyComplaintStatusVm">
    /// The status update information, including the daily complaint ID and the new status to be applied.
    /// </param>
    /// <returns>
    /// Returns the updated daily complaint status if successful;  
    /// otherwise, returns a <c>404 Not Found</c> response if the complaint does not exist.
    /// </returns>
    [Authorize(Roles = "Leader")]
    [HttpPost]
    public async Task<IActionResult> UpdateDailyComplaintStatusByLeader(DailyComplaintStatusVm dailyComplaintStatusVm)
    {
        var dailyComplaintStatus = await _dailyComplaintStatusService.UpdateDailyComplaintStatusByLeaderAsync(dailyComplaintStatusVm);

        if (dailyComplaintStatus == null)
            return NotFound($"No daily complaint found with complaintId {dailyComplaintStatusVm.DailyComplaintId}");
        return Ok(dailyComplaintStatus);
    }
}
