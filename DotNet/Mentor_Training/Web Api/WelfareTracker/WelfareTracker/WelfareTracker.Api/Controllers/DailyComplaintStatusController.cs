using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WelfareTracker.Core.Contracts.Service;
using WelfareTracker.Core.Vms;

namespace WelfareTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyComplaintStatusController(IDailyComplaintStatusService dailyComplaintStatusService) : ControllerBase
    {
        private readonly IDailyComplaintStatusService _dailyComplaintStatusService = dailyComplaintStatusService;

        [HttpPost]
        public async Task<IActionResult> AddDailyComplaintStatusByLeader(DailyComplaintStatusVm dailyComplaintStatusVm)
        {
            var dailyComplaintStatus = await _dailyComplaintStatusService.AddDailyComplaintStatusByLeader(dailyComplaintStatusVm);

            if (dailyComplaintStatus == null)
                return NotFound($"No daily complaint found with complaintId {dailyComplaintStatusVm.DailyComplaintId}");

            return Ok(dailyComplaintStatus);
        }
    }
}
