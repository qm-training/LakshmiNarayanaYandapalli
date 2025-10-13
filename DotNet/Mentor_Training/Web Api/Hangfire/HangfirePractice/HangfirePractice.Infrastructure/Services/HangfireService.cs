namespace HangfirePractice.Infrastructure.Services;
public class HangfireService(ILogger<HangfireService> logger) : IHangfireService
{
    private readonly ILogger<HangfireService> _logger = logger;
    public void CleanUpTempFiles()
    {
        _logger.LogInformation("Cleaning up temporary files...");

        Thread.Sleep(2000);

        _logger.LogInformation("Temporary files cleaned up.");
    }

    public void GenerateReport()
    {
        _logger.LogInformation("Generating report...");

        Thread.Sleep(3000);

        _logger.LogInformation("Report generated successfully.");
    }

    public void NotifyAdminAfterReport()
    {
        _logger.LogInformation("Notifying admin after report generation...");

        Thread.Sleep(1000);

        _logger.LogInformation("Admin notified successfully."); 
    }

    public void SendWelcomeEmail(string email)
    {
        _logger.LogInformation("Sending welcome email to {Email}...", email);

        Thread.Sleep(2000);

        _logger.LogInformation("Welcome email sent to {Email}.", email);
    }
}
