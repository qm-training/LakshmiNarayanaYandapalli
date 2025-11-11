namespace WelfareWorkTracker.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommentController (ICommentService commentService): ControllerBase
{
    private readonly ICommentService _commentService = commentService;

    [HttpPost("comment")]
    public async Task<IActionResult> AddCommentById(int? complaintId, int? dailyComplaintId, [FromBody] CommentVm commentVm)
    {
        var comment = await _commentService.AddCommentAsync(complaintId, dailyComplaintId, commentVm);

        if (comment == null && complaintId.HasValue)
        {
            return NotFound($"Complaint not found with complaintId: {complaintId}");
        }
        else if (comment == null && dailyComplaintId.HasValue)
        {
            return NotFound($"Daily Complaint not found with daily complaintId: {dailyComplaintId}");
        }

        return Ok("comment added sucessfully");
    }
    
    [HttpGet("comments-by-id")]
    public async Task<IActionResult> GetCommentsById(int? complaintId, int? dailyComplaintId)
    {
        var comments = await _commentService.GetCommentsByIdAsync(complaintId, dailyComplaintId);
        return Ok(comments);
    }

    [HttpPut("{commentId}")]
    public async Task<IActionResult> UpdateCommentByCommentId(int commentId, CommentVm commentVm)
    {
        var comment = await _commentService.UpdateCommentByIdAsync(commentId, commentVm);
        if (comment == null)
            return NotFound("Comment Not Found.");
        else
            return NoContent();
    }

    [HttpDelete("{commentId}")]
    public async Task<IActionResult> DeleteCommentByCommentId(int commentId)
    {
        var isDeleted = await _commentService.DeleteCommentByCommentIdAsync(commentId);

        if (!isDeleted)
            return NotFound(new { message = "Comment not found." });
        return NoContent();
    }
}
