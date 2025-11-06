using Microsoft.AspNetCore.Mvc;
using WelfareWorkTracker.Core.Contracts.Service;
using WelfareWorkTracker.Core.Vms;

namespace WelfareWorkTracker.Api.Controllers
{
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
            var complaint = await _complaintService.GetComplaintByIdAsync(complaintId);
            if (complaint == null)
                return NotFound($"No Complaint found with complaintId: {complaintId}");

            return Ok(complaint);
        }


        [HttpPut("{complaintId}")]
        public async Task<IActionResult> UpdateComplaintByComplaintId(int complaintId, ComplaintVm complaintVm)
        {
            var complaint = await _complaintService.UpdateComplaintAsync(complaintId, complaintVm);
            if (complaint == null)
                return NotFound($"No Complaint found with complaintId: {complaintId}");

            return Ok(complaint);
        }

        [HttpDelete("{complaintId}")]
        public async Task<IActionResult> DeleteComplaintByComplaintId(int complaintId)
        {
            var complaint = await _complaintService.DeleteComplaintByIdAsync(complaintId);
            if (!complaint)
                return NotFound($"No Complaint found with complaintId: {complaintId}");

            return Ok(new { message = "Complaint deleted successfully", complaint });
        }


        [HttpGet("get-user-raised-complaints/{userId}")]
        public async Task<IActionResult> GetComplaintsByUserId(int userId)
        {
            var complaints = await _complaintService.GetComplaintsByUserAsync(userId);
            return Ok(complaints);
        }

        [HttpGet("getComplaints")]
        public async Task<IActionResult> GetComplaintsByConstituencyName(GetComplaintsVm getComplaintsVm)
        {
            var complaints = await _complaintService.GetComplaintsByConstituencyAndStatusAsync(getComplaintsVm);
            return Ok(complaints);
        }
    }
}
