using WelfareWorkTracker.Core.Models;
using WelfareWorkTracker.Core.Vms;

namespace WelfareWorkTracker.Core.Contracts.Service
{
    public interface INotificationService
    {
        Task<Notification?> CreateNotificationAsync(CreateNotificationVm notificationVm);
        Task<List<Notification>> GetAllNotificationsByUserAsync(int userId);
        Task<Notification> GetNotificationByIdAsync(int notificationId);
        Task<int> GetUnReadNotificationsCountAsync(int userId);
        Task<bool> MarkAsReadAsync(int userId, int notificationId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<bool> DeleteNotificationAsync(int userId, int notificationId);
        Task<bool> DeleteAllNotificationsAsync(int userId);
    }
}
