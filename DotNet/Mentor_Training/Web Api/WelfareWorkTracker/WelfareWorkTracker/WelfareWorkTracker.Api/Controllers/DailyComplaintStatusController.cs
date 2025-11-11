namespace WelfareWorkTracker.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class DailyComplaintStatusController (IDailyComplaintStatusService dailyComplaintStatusService) : ControllerBase
{
    private readonly IDailyComplaintStatusService _dailyComplaintStatusService = dailyComplaintStatusService;

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
