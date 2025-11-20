namespace WelfareWorkTracker.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ComplaintController(IComplaintService complaintService) : ControllerBase
{
    private readonly IComplaintService _complaintService = complaintService;

    /// <summary>
    /// Raises a new complaint submitted by a citizen.
    /// </summary>
    /// <param name="complaintVm">
    /// The complaint details including title, description, constituency, and any additional information.
    /// </param>
    /// <returns>
    /// Returns a success message along with the newly created complaint details.
    /// </returns>
    [HttpPost("raiseAComplaint")]
    public async Task<IActionResult> AddComplaint(ComplaintVm complaintVm)
    {
        var newComplaint = await _complaintService.AddComplaintAsync(complaintVm);
        return Ok(new { messsage = "Complaint added successfully", newComplaint });
    }

    /// <summary>
    /// Retrieves a complaint by its unique complaint ID.
    /// </summary>
    /// <param name="complaintId">
    /// The ID of the complaint to retrieve.
    /// </param>
    /// <returns>
    /// Returns the complaint details if found; otherwise, returns a not found result.
    /// </returns>
    [HttpGet("{complaintId}")]
    public async Task<IActionResult> GetComplaintByComplaintId(int complaintId)
    {
        var complaint = await _complaintService.GetComplaintByComplaintIdAsync(complaintId);
        if (complaint == null)
            return NotFound($"No Complaint found with complaintId: {complaintId}");

        return Ok(complaint);
    }

    /// <summary>
    /// Updates an existing complaint using its unique complaint ID.
    /// </summary>
    /// <param name="complaintId">
    /// The ID of the complaint to update.
    /// </param>
    /// <param name="complaintVm">
    /// The updated complaint details such as title, description, or constituency.
    /// </param>
    /// <returns>
    /// Returns the updated complaint if successful; otherwise, a not found result.
    /// </returns>
    [HttpPut("{complaintId}")]
    public async Task<IActionResult> UpdateComplaintByComplaintId(int complaintId, ComplaintVm complaintVm)
    {
        var complaint = await _complaintService.UpdateComplaintByComplaintIdAsync(complaintId, complaintVm);
        if (complaint == null)
            return NotFound($"No Complaint found with complaintId: {complaintId}");

        return Ok(complaint);
    }

    /// <summary>
    /// Deletes a complaint using its unique complaint ID.
    /// </summary>
    /// <param name="complaintId">
    /// The ID of the complaint to delete.
    /// </param>
    /// <returns>
    /// Returns a success message when deletion is completed; otherwise, a not found result.
    /// </returns>
    [HttpDelete("{complaintId}")]
    public async Task<IActionResult> DeleteComplaintByComplaintId(int complaintId)
    {
        var complaint = await _complaintService.DeleteComplaintByComplaintIdAsync(complaintId);
        if (!complaint)
            return NotFound($"No Complaint found with complaintId: {complaintId}");

        return Ok(new { message = "Complaint deleted successfully", complaint });
    }

    /// <summary>
    /// Retrieves all complaints raised by the currently authenticated user.
    /// </summary>
    /// <returns>
    /// Returns a list of complaints raised by the user.
    /// </returns>
    [HttpGet("get-user-raised-complaints")]
    public async Task<IActionResult> GetComplaintsByUserId()
    {
        var userId = User.GetUserIdAsInt();
        var complaints = await _complaintService.GetComplaintsByUserIdAsync(userId);
        return Ok(complaints);
    }

    /// <summary>
    /// Retrieves all complaints for a given constituency for admin representatives.
    /// </summary>
    /// <param name="constituencyName">
    /// The name of the constituency to filter complaints.
    /// </param>
    /// <returns>
    /// Returns a list of complaints raised within the specified constituency.
    /// </returns>
    [HttpGet("getComplaints")]
    public async Task<IActionResult> GetComplaintsByConstituencyName([FromQuery] string constituencyName)
    {
        var complaints = await _complaintService.GetComplaintsForAdminRepAsync(constituencyName);
        return Ok(complaints);
    }

    /// <summary>
    /// Retrieves the most recent complaints submitted in a specific constituency.
    /// </summary>
    /// <param name="constituencyName">
    /// The constituency name to find recent complaints.
    /// </param>
    /// <returns>
    /// Returns a list of recently added complaints for public viewing.
    /// </returns>
    [AllowAnonymous]
    [HttpGet("recent-complaints")]
    public async Task<IActionResult> GetRecentComplaintsAsync([FromQuery] string constituencyName)
    {
        var recentComplaints = await _complaintService.GetRecentComplaintsAsync(constituencyName);
        return Ok(recentComplaints);
    }
}
