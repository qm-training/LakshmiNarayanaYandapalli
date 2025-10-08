namespace HangfirePractice.Core.Contracts.Services;
public interface IHangfireService
{
    void SendWelcomeEmail(string email);
    void GenerateReport();
    void CleanUpTempFiles();
    void NotifyAdminAfterReport();
}
