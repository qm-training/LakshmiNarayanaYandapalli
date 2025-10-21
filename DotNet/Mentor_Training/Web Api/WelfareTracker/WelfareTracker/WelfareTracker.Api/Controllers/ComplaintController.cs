using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WelfareTracker.Core.Contracts.Service;
using WelfareTracker.Core.Vms;

namespace WelfareTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintController(IComplaintService complaintService) : ControllerBase
    {
        private readonly IComplaintService _complaintService = complaintService;

        [HttpPost("register-complaint")]
        public async Task<IActionResult> RegisterComplaint(ComplaintVm complaintVm)
        {
            var result = await _complaintService.AddComplaintAsync(complaintVm);
            return Ok(new { message = "Complaint added successfully", result });
        }

        [HttpGet("{complaintId}")]
        public async Task<IActionResult> GetComplaintByComplaintId(int complaintId)
        {
            var result = await _complaintService.GetComplaintByComplaintIdAsync(complaintId);
            if (result == null)
            {
                return NotFound(new { message = "Complaint not found" });
            }
            return Ok(result);
        }

        [HttpGet("by-citizen/{citizenId}")]
        public async Task<IActionResult> GetComplaintsByUserId(int citizenId)
        {
            var result = await _complaintService.GetComplaintsByCitizenIdAsync(citizenId);
            if (result == null)
            {
                return NotFound(new { message = "No complaints found for this user" });
            }
            return Ok(result);
        }

        [HttpPut("update/{complaintId}")]
        public async Task<ActionResult> UpdateComplaint(int complaintId, ComplaintVm complaintVm)
        {
            var result = await _complaintService.UpdateComplaintAsync(complaintId, complaintVm);
            if (result == null)
            {
                return NotFound(new { message = "Complaint not found" });
            }

            return Ok(new { message = "Complaint updated successfully", result });
        }

        [HttpDelete("delete/{complaintId}")]
        public async Task<IActionResult> DeleteComplaint(int complaintId)
        {
            var result = await _complaintService.DeleteComplaintAsync(complaintId);
            if (!result)
            {
                return NotFound(new { message = "Complaint not found" });
            }
            return Ok(new { message = "Complaint deleted successfully", result });
        }

        [HttpGet("complaints-for-leader/{leaderId}")]
        public async Task<IActionResult> GetComplaintsForLeader(int leaderId)
        {
            var result = await _complaintService.GetLeaderComplaintsByLeaderId(leaderId);
            if (result == null)
            {
                return NotFound(new { message = "No complaints found for this leader" });
            }
            return Ok(result);
        }

    }
}
