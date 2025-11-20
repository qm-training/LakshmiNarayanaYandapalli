namespace WelfareWorkTracker.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class FeedbackController(IFeedbackService feedbackService) : ControllerBase
{
    private readonly IFeedbackService _feedbackService = feedbackService;

    /// <summary>
    /// Adds feedback for a complaint or daily complaint submitted by a citizen.
    /// </summary>
    /// <param name="feedbackVm">The feedback information including complaint ID or daily complaint ID, 
    /// satisfaction status, and feedback message.</param>
    /// <returns>
    /// Returns a success message if added successfully; otherwise returns a bad request response.
    /// </returns>
    [Authorize(Roles = "Citizen")]
    [HttpPost]
    public async Task<IActionResult> AddFeedback(FeedbackVm feedbackVm)
    {
        var citizenFeedback = await _feedbackService.AddFeedbackAsync(feedbackVm);
        if (citizenFeedback == null)
            return BadRequest("Failed to add feedback, Please retry!");
        return Ok("Feedback added sucessfully");
    }

    /// <summary>
    /// Retrieves feedback associated with a complaint or daily complaint.
    /// </summary>
    /// <param name="complaintId">The ID of the complaint for which feedback is requested. Optional.</param>
    /// <param name="dailyComplaintId">The ID of the daily complaint for which feedback is requested. Optional.</param>
    /// <param name="isAllComplaints">
    /// Indicates whether to fetch all feedback entries (<c>true</c>) or only the feedback submitted 
    /// by the currently logged-in user (<c>false</c>).
    /// </param>
    /// <returns>
    /// Returns a list of feedback entries.  
    /// If a single-user query is requested and no feedback exists, returns <c>NotFound</c>.
    /// </returns>
    [HttpGet("feedback")]
    public async Task<IActionResult> GetFeedbackById(int? complaintId, int? dailyComplaintId, bool isAllComplaints)
    {
        List<FeedbackDto> complaintsFeedbackDtos;
        if (isAllComplaints)
        {
            complaintsFeedbackDtos = await _feedbackService.GetAllFeedbacksAsync(complaintId, dailyComplaintId) ?? [];
        }
        else
        {
            var result = await _feedbackService.GetFeedbackByUserAsync(complaintId, dailyComplaintId);
            if (result == null)
            {
                return NotFound();
            }
            List<FeedbackDto> complaintFeedbackDtos = [result!];
            complaintsFeedbackDtos = complaintFeedbackDtos;
        }

        return Ok(complaintsFeedbackDtos);
    }
}
