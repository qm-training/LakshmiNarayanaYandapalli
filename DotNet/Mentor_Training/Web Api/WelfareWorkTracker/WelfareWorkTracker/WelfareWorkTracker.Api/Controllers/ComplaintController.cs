namespace WelfareWorkTracker.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ComplaintController(IComplaintService complaintService) : ControllerBase
{
    private readonly IComplaintService _complaintService = complaintService;

    [HttpPost("raiseAComplaint")]
    public async Task<IActionResult> AddComplaint(ComplaintVm complaintVm)
    {
        var newComplaint = await _complaintService.AddComplaintAsync(complaintVm);
        return Ok(new { messsage = "Complaint added successfully", newComplaint });
    }

    [HttpGet("{complaintId}")]
    public async Task<IActionResult> GetComplaintByComplaintId(int complaintId)
    {
        var complaint = await _complaintService.GetComplaintByComplaintIdAsync(complaintId);
        if (complaint == null)
            return NotFound($"No Complaint found with complaintId: {complaintId}");

        return Ok(complaint);
    }

    [HttpPut("{complaintId}")]
    public async Task<IActionResult> UpdateComplaintByComplaintId(int complaintId, ComplaintVm complaintVm)
    {
        var complaint = await _complaintService.UpdateComplaintByComplaintIdAsync(complaintId, complaintVm);
        if (complaint == null)
            return NotFound($"No Complaint found with complaintId: {complaintId}");

        return Ok(complaint);
    }

    [HttpDelete("{complaintId}")]
    public async Task<IActionResult> DeleteComplaintByComplaintId(int complaintId)
    {
        var complaint = await _complaintService.DeleteComplaintByComplaintIdAsync(complaintId);
        if (!complaint)
            return NotFound($"No Complaint found with complaintId: {complaintId}");

        return Ok(new { message = "Complaint deleted successfully", complaint });
    }

    [HttpGet("get-user-raised-complaints")]
    public async Task<IActionResult> GetComplaintsByUserId()
    {
        var userId = User.GetUserIdAsInt();
        var complaints = await _complaintService.GetComplaintsByUserIdAsync(userId);
        return Ok(complaints);
    }

    [HttpGet("getComplaints")]
    public async Task<IActionResult> GetComplaintsByConstituencyName([FromQuery] string constituencyName)
    {
        var complaints = await _complaintService.GetComplaintsForAdminRepAsync(constituencyName);
        return Ok(complaints);
    }

    [AllowAnonymous]
    [HttpGet("recent-complaints")]
    public async Task<IActionResult> GetRecentComplaintsAsync([FromQuery] string constituencyName)
    {
        var recentComplaints = await _complaintService.GetRecentComplaintsAsync(constituencyName);
        return Ok(recentComplaints);
    }
}
