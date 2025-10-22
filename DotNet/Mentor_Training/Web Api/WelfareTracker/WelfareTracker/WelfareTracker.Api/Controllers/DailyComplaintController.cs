using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WelfareTracker.Core.Contracts.Service;
using WelfareTracker.Core.Vms;

namespace WelfareTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyComplaintController(IDailyComplaintService dailyComplaintService) : ControllerBase
    {
        private readonly IDailyComplaintService _dailyComplaintService = dailyComplaintService;

        [HttpGet("{dailyComplaintId}")]
        public async Task<IActionResult> GetDailyComplaintById(int dailyComplaintId)
        {
            var dailyComplaint = await _dailyComplaintService.GetDailyComplaintByIdAsync(dailyComplaintId);

            if (dailyComplaint == null)
                return NotFound($"Daily complaint not found with id: {dailyComplaintId}");

            return Ok(dailyComplaint);
        }

        [HttpPut("update-dailyComplaint/{dailyComplaintId}")]
        public async Task<IActionResult> UpdateDailyComplaint(int dailyComplaintId, DailyComplaintVm dailyComplaintVm)
        {
            var dailyComplaint = await _dailyComplaintService.UpdateDailyComplaintAsync(dailyComplaintId, dailyComplaintVm);

            if (dailyComplaint == null)
                return NotFound("No Daily Complaint found with Id: {id}");

            return Ok(dailyComplaint);
        }

        [HttpGet("get-today-daily-complaints")]
        public async Task<IActionResult> GetTodayDailyComplaints()
        {
            var dailyComplaints = await _dailyComplaintService.GetTodaysDailyComplaintsAsync();
            return Ok(dailyComplaints);
        }

        [HttpGet("leader-daily-complaint/{ledaerId}")]
        public async Task<IActionResult> GetDailyComplaintForLeader(int leaderId)
        {
            var dailyComplaintOfaleader = await _dailyComplaintService.GetDailyComplaintByLeaderIdAsync(leaderId);
            if (dailyComplaintOfaleader == null)
                return NotFound();
            return Ok(dailyComplaintOfaleader);
        }
    }
}
