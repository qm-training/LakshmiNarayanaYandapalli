using Hangfire;
using HangfirePractice.Core.Contracts.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HangfirePractice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HangfireController(IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager) : ControllerBase
    {
        private readonly IBackgroundJobClient _backgroundJobClient = backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager = recurringJobManager;


        [HttpPost("send-welcome")]
        public IActionResult SendWelcomeEmail([FromQuery] string email)
        {
            _backgroundJobClient.Enqueue<IHangfireService>(x => x.SendWelcomeEmail(email));
            return Ok($"Welcome email job queued for {email}!");
        }

        [HttpPost("generate-report")]
        public IActionResult GenerateReport()
        {
            _backgroundJobClient.Schedule<IHangfireService>(
                x => x.GenerateReport(),
                TimeSpan.FromMinutes(2)
            );

            return Ok("Report generation job scheduled after 2 minutes.");
        }

        [HttpPost("schedule-cleanup")]
        public IActionResult ScheduleCleanup()
        {
            _recurringJobManager.AddOrUpdate<IHangfireService>(
                "daily-cleanup-job",
                x => x.CleanUpTempFiles(),
                Cron.Minutely
            );

            return Ok("Daily cleanup job scheduled successfully!");
        }

        [HttpPost("chain-report-notify")]
        public IActionResult ChainReportAndNotify()
        {
            var reportJobId = _backgroundJobClient.Enqueue<IHangfireService>(x => x.GenerateReport());
            _backgroundJobClient.ContinueJobWith<IHangfireService>(reportJobId, x => x.NotifyAdminAfterReport());
            return Ok("Chained job: report generation followed by admin notification.");
        }
    }
}
