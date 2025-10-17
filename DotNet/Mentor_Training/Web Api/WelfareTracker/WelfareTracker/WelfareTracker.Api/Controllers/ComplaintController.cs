using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WelfareTracker.Core.Contracts.Service;
using WelfareTracker.Core.Vms;

namespace WelfareTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintController (IComplaintService complaintService) : ControllerBase
    {
        private readonly IComplaintService _complaintService = complaintService;

        [HttpPost("register-complaint")]
        public async Task<IActionResult> RegisterComplaint([FromBody] ComplaintVm complaintVm)
        {
            var result = await _complaintService.AddComplaintAsync(complaintVm);
            return Ok(new { message = "Complaint added successfully", result});
        }

        [HttpGet]
        public async Task<IActionResult> GetComplaintByComplaintId([FromQuery] int complaintId)
        {
            var result = await _complaintService.GetComplaintByComplaintIdAsync(complaintId);
            if (result == null)
            {
                return NotFound(new { message = "Complaint not found" });
            }
            return Ok(result);
        }

        [HttpGet("by-citizen")]
        public async Task<IActionResult> GetComplaintsByUserId([FromQuery] int userId)
        {
            var result = await _complaintService.GetComplaintsByCitizenIdAsync(userId);
            if (result == null)
            {
                return NotFound(new { message = "No complaints found for this user" });
            }
            return Ok(result);
        }

    }
}
