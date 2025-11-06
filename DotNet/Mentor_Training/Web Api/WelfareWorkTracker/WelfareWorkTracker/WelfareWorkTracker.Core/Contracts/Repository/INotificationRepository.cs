using WelfareWorkTracker.Core.Models;

namespace WelfareWorkTracker.Core.Contracts.Repository
{
    public interface INotificationRepository
    {
        Task<Notification?> CreateNotificationAsync(Notification notification);
        Task<bool> DeleteAllNotificationsAsync(int userId);
        Task<bool> DeleteNotificationAsync(int notificationId);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<List<Notification>> GetAllNotificationsByUserAsync(int userId);
        Task<Notification?> GetNotificationByIdAsync(int notificationId);
        Task<int> GetUnReadNotificationsCountAsync(int userId);
    }
}
