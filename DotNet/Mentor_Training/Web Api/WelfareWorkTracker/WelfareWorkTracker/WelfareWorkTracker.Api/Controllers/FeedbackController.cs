namespace WelfareWorkTracker.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class FeedbackController(IFeedbackService feedbackService) : ControllerBase
{
    private readonly IFeedbackService _feedbackService = feedbackService;

    [Authorize(Roles = "Citizen")]
    [HttpPost]
    public async Task<IActionResult> AddFeedback(FeedbackVm feedbackVm)
    {
        var citizenFeedback = await _feedbackService.AddFeedbackAsync(feedbackVm);
        if (citizenFeedback == null)
            return BadRequest("Failed to add feedback, Please retry!");
        return Ok("Feedback added sucessfully");
    }

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
