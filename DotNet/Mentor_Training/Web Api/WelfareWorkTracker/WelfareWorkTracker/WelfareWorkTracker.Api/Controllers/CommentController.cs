namespace WelfareWorkTracker.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommentController (ICommentService commentService): ControllerBase
{
    private readonly ICommentService _commentService = commentService;
    
    /// <summary>
    /// Adds a new comment for a complaint or daily complaint.
    /// </summary>
    /// <param name="complaintId">
    /// The ID of the complaint to which the comment should be added. Optional.
    /// </param>
    /// <param name="dailyComplaintId">
    /// The ID of the daily complaint to which the comment should be added. Optional.
    /// </param>
    /// <param name="commentVm">
    /// The comment details including description and anonymity flag.
    /// </param>
    /// <returns>
    /// Returns a success message if the comment is added; otherwise, returns a not found result.
    /// </returns>
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

    /// <summary>
    /// Retrieves all comments associated with a complaint or daily complaint.
    /// </summary>
    /// <param name="complaintId">
    /// The ID of the complaint whose comments are being requested. Optional.
    /// </param>
    /// <param name="dailyComplaintId">
    /// The ID of the daily complaint whose comments are being requested. Optional.
    /// </param>
    /// <returns>
    /// Returns a list of comments for the specified complaint or daily complaint.
    /// </returns>
    [HttpGet("comments-by-id")]
    public async Task<IActionResult> GetCommentsById(int? complaintId, int? dailyComplaintId)
    {
        var comments = await _commentService.GetCommentsByIdAsync(complaintId, dailyComplaintId);
        return Ok(comments);
    }

    /// <summary>
    /// Updates an existing comment by its comment ID.
    /// </summary>
    /// <param name="commentId">
    /// The unique identifier of the comment to be updated.
    /// </param>
    /// <param name="commentVm">
    /// The updated comment data such as description or anonymity flag.
    /// </param>
    /// <returns>
    /// Returns NoContent if the update is successful; otherwise, a not found result.
    /// </returns>
    [HttpPut("{commentId}")]
    public async Task<IActionResult> UpdateCommentByCommentId(int commentId, CommentVm commentVm)
    {
        var comment = await _commentService.UpdateCommentByIdAsync(commentId, commentVm);
        if (comment == null)
            return NotFound("Comment Not Found.");
        else
            return NoContent();
    }

    /// <summary>
    /// Deletes a comment using its unique comment ID.
    /// </summary>
    /// <param name="commentId">
    /// The ID of the comment to be deleted.
    /// </param>
    /// <returns>
    /// Returns NoContent if the deletion is successful; otherwise, a not found result.
    /// </returns>
    [HttpDelete("{commentId}")]
    public async Task<IActionResult> DeleteCommentByCommentId(int commentId)
    {
        var isDeleted = await _commentService.DeleteCommentByCommentIdAsync(commentId);

        if (!isDeleted)
            return NotFound(new { message = "Comment not found." });
        return NoContent();
    }
}
