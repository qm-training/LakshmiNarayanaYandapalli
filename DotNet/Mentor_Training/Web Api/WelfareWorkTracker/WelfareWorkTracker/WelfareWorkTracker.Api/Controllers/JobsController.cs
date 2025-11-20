namespace WelfareWorkTracker.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JobsController : ControllerBase
{
    /// <summary>
    /// Schedules a recurring Hangfire job to assign daily complaints to leaders.
    /// </summary>
    /// <remarks>
    /// This job runs every day at 12:00 AM. It triggers <see cref="IDailyComplaintService.AssignDailyComplaintsAsync"/>.
    /// </remarks>
    /// <returns>Returns a confirmation message when the job is scheduled.</returns>
    [HttpPost("startAssigningDailyComplaints")]
    [ProducesResponseType(typeof(string), 200)]
    public IActionResult StartJobAssignDailyComplaints()
    {
        RecurringJob.AddOrUpdate<IDailyComplaintService>(
            "AssignComplaintsToLeaders",
            service => service.AssignDailyComplaintsAsync(),
            "0 0 * * *"); // Runs daily at 12:00 AM
        return Ok("Job scheduled to run daily at 12:00 AM.");
    }

    /// <summary>
    /// Stops the recurring job responsible for assigning daily complaints.
    /// </summary>
    /// <returns>Returns a confirmation message when the job is stopped.</returns>
    [HttpPost("stopAssigningDailyComplaints")]
    [ProducesResponseType(typeof(string), 200)]
    public IActionResult StopJobAssignDailyComplaints()
    {
        RecurringJob.RemoveIfExists("AssignComplaintsToLeaders");
        return Ok("Job has been stopped.");
    }

    /// <summary>
    /// Schedules a recurring Hangfire job to evaluate feedback on daily complaints.
    /// </summary>
    /// <remarks>
    /// This job runs once every day. It triggers <see cref="IFeedbackService.EvaluateDailyComplaintFeedback"/>.
    /// </remarks>
    /// <returns>Returns a confirmation message when the job is scheduled.</returns>
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

    /// <summary>
    /// Stops the recurring job that evaluates daily complaint feedback.
    /// </summary>
    /// <returns>Returns a confirmation message when the job is stopped.</returns>
    [HttpPost("stopEvaluatingDailyComplaintFeedback")]
    [ProducesResponseType(typeof(string), 200)]
    public IActionResult StopJobEvaluateDailyComplaintFeedback()
    {
        RecurringJob.RemoveIfExists("EvaluateDailyComplaintsFeedback");
        return Ok("Job has been stopped.");
    }

    /// <summary>
    /// Schedules a recurring Hangfire job to check the status of daily complaints.
    /// </summary>
    /// <remarks>
    /// This job runs every day at 12:00 PM and triggers <see cref="IDailyComplaintStatusService.CheckDailyComplaintStatusAsync"/>.
    /// </remarks>
    /// <returns>Returns a confirmation message when the job is scheduled.</returns>
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

    /// <summary>
    /// Stops the recurring job that checks daily complaint statuses.
    /// </summary>
    /// <returns>Returns a confirmation message when the job is stopped.</returns>
    [HttpPost("stopCheckingDailyComplaintStatus")]
    [ProducesResponseType(typeof(string), 200)]
    public IActionResult StopJobCheckDailyComplaintStatus()
    {
        RecurringJob.RemoveIfExists("CheckDailyComplaintStatus");
        return Ok("Job has been stopped.");
    }

    /// <summary>
    /// Schedules a recurring Hangfire job to check complaints that require leader approval.
    /// </summary>
    /// <remarks>
    /// This job runs every 6 hours and triggers <see cref="IComplaintService.CheckComplaintsForLeaderApprovalAsync"/>.
    /// </remarks>
    /// <returns>Returns a confirmation message when the job is scheduled.</returns>
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

    /// <summary>
    /// Stops the recurring job responsible for checking complaints awaiting leader approval.
    /// </summary>
    /// <returns>Returns a confirmation message when the job is stopped.</returns>
    [HttpPost("stopCheckingComplaintsForLeaderApproval")]
    [ProducesResponseType(typeof(string), 200)]
    public IActionResult StopJobCheckComplaintsForLeaderApproval()
    {
        RecurringJob.RemoveIfExists("CheckComplaintsForApproval");
        return Ok("Job has been stopped.");
    }
}
