namespace WelfareWorkTracker.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DailyComplaintController(IDailyComplaintService dailyComplaintService) : ControllerBase
{
    private readonly IDailyComplaintService _dailyComplaintService = dailyComplaintService;

    /// <summary>
    /// Retrieves a daily complaint by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the daily complaint.</param>
    /// <returns>
    /// Returns the daily complaint details if found; otherwise, returns a <c>404 Not Found</c> response.
    /// </returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDailyComplaintsById(int id)
    {
        var dailyComplaint = await _dailyComplaintService.GetDailyComplaintByIdAsync(id);
        if (dailyComplaint == null)
            return NotFound($"No Daily Complaint found with complaintId: {id}");
        return Ok(dailyComplaint);
    }

    /// <summary>
    /// Updates an existing daily complaint using the provided information.
    /// </summary>
    /// <param name="id">The unique identifier of the daily complaint to update.</param>
    /// <param name="dailyComplaintVm">The updated daily complaint details.</param>
    /// <returns>
    /// Returns the updated complaint if successful; otherwise, returns a <c>404 Not Found</c> response.
    /// </returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDailyComplaintById(int id, DailyComplaintVm dailyComplaintVm)
    {
        var dailyComplaint = await _dailyComplaintService.UpdateDailyComplaintAsync(id, dailyComplaintVm);
        if (dailyComplaint == null)
            return NotFound($"No Daily Complaint found with complaintId: {id}");
        return Ok(dailyComplaint);
    }

    /// <summary>
    /// Retrieves all daily complaints available in the system.
    /// </summary>
    /// <returns>
    /// Returns a list of daily complaints.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetDailyComplaints()
    {
        var dailyComplaints = await _dailyComplaintService.GetDailyComplaintsAsync();
        return Ok(dailyComplaints);
    }

    /// <summary>
    /// Retrieves daily complaints assigned to a specific leader.
    /// </summary>
    /// <param name="leaderId">The unique identifier of the leader.</param>
    /// <returns>
    /// Returns a list of daily complaints assigned to the leader;  
    /// otherwise, returns a <c>404 Not Found</c> response.
    /// </returns>
    [HttpGet("leader/{leaderId}")]
    public async Task<IActionResult> GetDailyComplaintByLeaderId(int leaderId)
    {
        var dailyComplaintOfaleader = await _dailyComplaintService.GetDailyComplaintByLeaderIdAsync(leaderId);
        if (dailyComplaintOfaleader == null)
            return NotFound();
        return Ok(dailyComplaintOfaleader);
    }

    /// <summary>
    /// Retrieves all daily complaints belonging to a specific constituency.
    /// </summary>
    /// <param name="constituencyName">The name of the constituency.</param>
    /// <returns>
    /// Returns the list of daily complaints for the constituency;  
    /// otherwise, returns <c>404 Not Found</c> if none exist.
    /// </returns>
    [HttpGet("constituency/{constituencyName}")]
    public async Task<IActionResult> GetDailyComplaintByConstituency(string constituencyName)
    {
        var dailyComplaint = await _dailyComplaintService.GetDailyComplaintByConstituencyNameAsync(constituencyName);

        if (dailyComplaint == null)
            return NotFound($"No daily complaints found for constituency: {constituencyName}");
        return Ok(dailyComplaint);
    }
}
