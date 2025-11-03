using Hangfire;
using Microsoft.AspNetCore.Mvc;
using WelfareWorkTracker.Core.Contracts.Service;

namespace WelfareWorkTracker.Api.Controllers
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

        [HttpPost("startEvaluatingDailyComplaintFeedback")]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult StartJobEvaluateDailyComplaintFeedback()
        {
            RecurringJob.AddOrUpdate<IFeedbackService>(
        "EvaluateDailyComplaintsFeedback",
        service => service.EvaluateDailyComplaintFeedback(),
        Cron.Daily); // Run daily
            return Ok("Job scheduled to run daily at 12:00 AM.");
        }

        [HttpPost("stopEvaluatingDailyComplaintFeedback")]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult StopJobEvaluateDailyComplaintFeedback()
        {
            RecurringJob.RemoveIfExists("EvaluateDailyComplaintsFeedback");
            return Ok("Job has been stopped.");
        }

        [HttpPost("startCheckingDailyComplaintStatus")]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult StartJobCheckDailyComplaintStatus()
        {
            RecurringJob.AddOrUpdate<IDailyComplaintStatusService>(
        "CheckDailyComplaintStatus",
        service => service.CheckDailyComplaintStatusAsync(),
        "0 12 * * *");  // This cron expression runs at 12:00 PM daily
            return Ok("Job scheduled to run daily at 12:00 PM.");
        }

        [HttpPost("stopCheckingDailyComplaintStatus")]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult StopJobCheckDailyComplaintStatus()
        {
            RecurringJob.RemoveIfExists("CheckDailyComplaintStatus");
            return Ok("Job has been stopped.");
        }

        [HttpPost("startCheckingComplaintsForLeaderApproval")]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult StartJobCheckComplaintsForLeaderApproval()
        {
            RecurringJob.AddOrUpdate<IComplaintService>(
                "CheckComplaintsForApproval",
                service => service.CheckComplaintsForLeaderApprovalAsync(),
                "0 */6 * * *");
            return Ok("Job scheduled to run every 6 hrs.");
        }

        [HttpPost("stopCheckingComplaintsForLeaderApproval")]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult StopJobCheckComplaintsForLeaderApproval()
        {
            RecurringJob.RemoveIfExists("CheckComplaintsForApproval");
            return Ok("Job has been stopped.");
        }
    }
}
