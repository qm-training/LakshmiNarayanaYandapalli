using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WelfareTracker.Core.Contracts.Service;
using WelfareTracker.Core.Vms;

namespace WelfareTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController(ICommentService commentService) : ControllerBase
    {
        private readonly ICommentService _commentService = commentService;

        [HttpPost("add-comment")]
        public async Task<IActionResult> AddCommentAsync(CommentVm commentVm)
        {
            var result = await _commentService.AddComment(commentVm);
            if (result == null)
                return BadRequest("Unable to add comment.");

            return Ok(new { message = "comment added successfully", result });
        }

        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateCommentAsync(int commentId, CommentVm commentVm)
        {
            var result = await _commentService.UpdateCommentByIdAsync(commentId, commentVm);
            if (result == null)
                return BadRequest("Unable to update comment.");

            return Ok(new { message = "comment updated successfully", result });
        }

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteCommentAsync(int commentId)
        {
            var result = await _commentService.DeleteCommentByIdAsync(commentId);
            if (!result)
                return BadRequest("Unable to delete comment.");

            return Ok(new { message = "comment deleted successfully", result });
        }

        [HttpGet("{citizenId}")]
        public async Task<IActionResult> GetCommentsByCitizenId(int citizenId)
        {
            var result = await _commentService.GetCommentsByCitizenId(citizenId);
            if (result == null)
                return NotFound("No comments found for citizen");

            return Ok(result);
        }

        [HttpPost("{complaintId}")]
        public async Task<IActionResult> GetCommentsByComplaintId(int complaintId)
        {
            var result = await _commentService.GetCommentsByComplaintId(complaintId);
            if (result == null)
                return NotFound("No comments found for citizen");

            return Ok(result);
        }
    }
}
