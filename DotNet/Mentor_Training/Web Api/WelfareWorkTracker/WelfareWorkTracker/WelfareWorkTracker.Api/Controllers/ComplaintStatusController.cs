namespace WelfareWorkTracker.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ComplaintStatusController(IComplaintStatusService complaintStatusService) : ControllerBase
{
    private readonly IComplaintStatusService _complaintStatusService = complaintStatusService;

    /// <summary>
    /// Retrieves the complete status history for a specific complaint.
    /// </summary>
    /// <param name="complaintId">
    /// The unique identifier of the complaint whose status history is requested.
    /// </param>
    /// <returns>
    /// Returns the full list of status updates for the complaint; 
    /// otherwise, a not found result if no history exists.
    /// </returns>
    [HttpGet("status-history/{complaintId}")]
    public async Task<IActionResult> GetComplaintStatusHistory(int complaintId)
    {
        var statusHistory = await _complaintStatusService.GetComplaintStatusHistoryAsync(complaintId);

        if (statusHistory?.Count == 0)
            return NotFound("No status history found for the specified complaint.");

        return Ok(statusHistory);
    }

    /// <summary>
    /// Retrieves the most recent status of a specified complaint.
    /// </summary>
    /// <param name="complaintId">
    /// The ID of the complaint whose latest status is being requested.
    /// </param>
    /// <returns>
    /// Returns the most recent complaint status; 
    /// otherwise, a not found result if the complaint does not exist.
    /// </returns>
    [HttpGet("recentStatus/{complaintId}")]
    public async Task<IActionResult> GetComplaintStatusByComplaintId(int complaintId)
    {
        var status = await _complaintStatusService.GetComplaintStatusByComplaintId(complaintId);
        if (status == null)
            return NotFound($" No complaint found with complaintId {complaintId}");

        return Ok(status);
    }

    /// <summary>
    /// Retrieves a complaint status by its unique status record ID.
    /// </summary>
    /// <param name="complaintStatusId">
    /// The unique identifier of the complaint status record.
    /// </param>
    /// <returns>
    /// Returns the complaint status if found; 
    /// otherwise, a not found response.
    /// </returns>
    [Authorize(Roles = "Admin")]
    [HttpGet("{complaintStatusId}")]
    public async Task<IActionResult> GetComplaintStatusById(int complaintStatusId)
    {
        var status = await _complaintStatusService.GetComplaintStatusByIdAsync(complaintStatusId);
        if (status == null)
            return NotFound($" No complaint found with complaintId {complaintStatusId}");

        return Ok(status);
    }

    /// <summary>
    /// Allows a leader to update the status of a complaint assigned to them.
    /// </summary>
    /// <param name="statusByLeaderVm">
    /// The updated complaint status details provided by the leader.
    /// </param>
    /// <returns>
    /// Returns the updated status if successful; otherwise, a not found result.
    /// </returns>
    [Authorize(Roles = "Leader")]
    [HttpPost("updateComplaintStatusByLeader")]
    public async Task<IActionResult> UpdateComplaintStatusByLeader(StatusByLeaderVm statusByLeaderVm)
    {
        var status = await _complaintStatusService.UpdateComplaintStatusByLeaderAsync(statusByLeaderVm);
        if (status == null)
            return NotFound($"No complaint found with complaintId {statusByLeaderVm.ComplaintId}");
        return Ok(status);
    }

    /// <summary>
    /// Validates or updates a complaint’s status by the Admin Representative.
    /// </summary>
    /// <param name="statusByAdminRepVm">
    /// The complaint validation details provided by the Admin Representative.
    /// </param>
    /// <returns>
    /// Returns the updated complaint status if successful; otherwise, a not found response.
    /// </returns>
    [Authorize(Roles = "AdminRepresentative")]
    [HttpPost("validateByAdminRep")]
    public async Task<IActionResult> VerifyComplaintByAdminRep(StatusByAdminRepVm statusByAdminRepVm)
    {
        var result = await _complaintStatusService.AddComplaintStatusByAdminRepAsync(statusByAdminRepVm);
        if (result == null)
            return NotFound($"Complaint does not exists with complaintId:{statusByAdminRepVm.ComplaintId}");
        return Ok(result);
    }

    /// <summary>
    /// Updates or validates the complaint status as an Admin.
    /// </summary>
    /// <param name="statusByAdminVm">
    /// The status update information provided by the Admin.
    /// </param>
    /// <returns>
    /// Returns the updated complaint status if successful; otherwise, a not found result.
    /// </returns>
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
