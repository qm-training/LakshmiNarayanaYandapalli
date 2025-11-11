namespace WelfareWorkTracker.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DailyComplaintController(IDailyComplaintService dailyComplaintService) : ControllerBase
{
    private readonly IDailyComplaintService _dailyComplaintService = dailyComplaintService;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDailyComplaintsById(int id)
    {
        var dailyComplaint = await _dailyComplaintService.GetDailyComplaintByIdAsync(id);
        if (dailyComplaint == null)
            return NotFound($"No Daily Complaint found with complaintId: {id}");
        return Ok(dailyComplaint);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDailyComplaintById(int id, DailyComplaintVm dailyComplaintVm)
    {
        var dailyComplaint = await _dailyComplaintService.UpdateDailyComplaintAsync(id, dailyComplaintVm);
        if (dailyComplaint == null)
            return NotFound($"No Daily Complaint found with complaintId: {id}");
        return Ok(dailyComplaint);
    }

    [HttpGet]
    public async Task<IActionResult> GetDailyComplaints()
    {
        var dailyComplaints = await _dailyComplaintService.GetDailyComplaintsAsync();
        return Ok(dailyComplaints);
    }

    [HttpGet("leader/{leaderId}")]
    public async Task<IActionResult> GetDailyComplaintByLeaderId(int leaderId)
    {
        var dailyComplaintOfaleader = await _dailyComplaintService.GetDailyComplaintByLeaderIdAsync(leaderId);
        if (dailyComplaintOfaleader == null)
            return NotFound();
        return Ok(dailyComplaintOfaleader);
    }

    [HttpGet("constituency/{constituencyName}")]
    public async Task<IActionResult> GetDailyComplaintByConstituency(string constituencyName)
    {
        var dailyComplaint = await _dailyComplaintService.GetDailyComplaintByConstituencyNameAsync(constituencyName);

        if (dailyComplaint == null)
            return NotFound($"No daily complaints found for constituency: {constituencyName}");
        return Ok(dailyComplaint);
    }
}
