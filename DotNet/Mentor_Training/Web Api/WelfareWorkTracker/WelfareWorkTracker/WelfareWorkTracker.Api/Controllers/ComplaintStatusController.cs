namespace WelfareWorkTracker.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ComplaintStatusController(IComplaintStatusService complaintStatusService) : ControllerBase
{
    private readonly IComplaintStatusService _complaintStatusService = complaintStatusService;

    [HttpGet("status-history/{complaintId}")]
    public async Task<IActionResult> GetComplaintStatusHistory(int complaintId)
    {
        var statusHistory = await _complaintStatusService.GetComplaintStatusHistoryAsync(complaintId);

        if (statusHistory?.Count == 0)
            return NotFound("No status history found for the specified complaint.");

        return Ok(statusHistory);
    }

    [HttpGet("recentStatus/{complaintId}")]
    public async Task<IActionResult> GetComplaintStatusByComplaintId(int complaintId)
    {
        var status = await _complaintStatusService.GetComplaintStatusByComplaintId(complaintId);
        if (status == null)
            return NotFound($" No complaint found with complaintId {complaintId}");

        return Ok(status);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{complaintStatusId}")]
    public async Task<IActionResult> GetComplaintStatusById(int complaintStatusId)
    {
        var status = await _complaintStatusService.GetComplaintStatusByIdAsync(complaintStatusId);
        if (status == null)
            return NotFound($" No complaint found with complaintId {complaintStatusId}");

        return Ok(status);
    }

    [Authorize(Roles = "Leader")]
    [HttpPost("updateComplaintStatusByLeader")]
    public async Task<IActionResult> UpdateComplaintStatusByLeader(StatusByLeaderVm statusByLeaderVm)
    {
        var status = await _complaintStatusService.UpdateComplaintStatusByLeaderAsync(statusByLeaderVm);
        if (status == null)
            return NotFound($"No complaint found with complaintId {statusByLeaderVm.ComplaintId}");
        return Ok(status);
    }

    [Authorize(Roles = "AdminRepresentative")]
    [HttpPost("validateByAdminRep")]
    public async Task<IActionResult> VerifyComplaintByAdminRep(StatusByAdminRepVm statusByAdminRepVm)
    {
        var result = await _complaintStatusService.AddComplaintStatusByAdminRepAsync(statusByAdminRepVm);
        if (result == null)
            return NotFound($"Complaint does not exists with complaintId:{statusByAdminRepVm.ComplaintId}");
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("updateStatusByAdmin")]
    public async Task<IActionResult> VerifyComplaintByAdmin(StatusByAdminVm statusByAdminVm)
    {
        var result = await _complaintStatusService.AddComplaintStatusByAdminAsync(statusByAdminVm);
        if (result == null)
            return NotFound($"Complaint does not exists with complaintId:{statusByAdminVm.ComplaintId}");

        return Ok(result);
    }
}
