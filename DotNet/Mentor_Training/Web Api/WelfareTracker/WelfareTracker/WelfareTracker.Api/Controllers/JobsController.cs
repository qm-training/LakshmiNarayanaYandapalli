using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WelfareTracker.Core.Contracts.Service;

namespace WelfareTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        [HttpPost("startAssigningDailyComplaints")]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult StartJobAssignDailyComplaints()
        {
            RecurringJob.AddOrUpdate<IDailyComplaintService>(
                "AssignComplaintsToLeaders",
                service => service.AssignDailyComplaintsToLeadersAsync(),
                "0 0 * * *"); // Runs daily at 12:00 AM
            return Ok("Job scheduled to run daily at 12:00 AM.");
        }

        [HttpPost("stopAssigningDailyComplaints")]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult StopJobAssignDailyComplaints()
        {
            RecurringJob.RemoveIfExists("AssignComplaintsToLeaders");
            return Ok("Job has been stopped.");
        }


    }
}
