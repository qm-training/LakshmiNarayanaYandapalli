using Microsoft.EntityFrameworkCore;
using WelfareWorkTracker.Core.Contracts.Repository;
using WelfareWorkTracker.Core.Exceptions;
using WelfareWorkTracker.Core.Models;
using WelfareWorkTracker.Infrastructure.Data;

namespace WelfareWorkTracker.Infrastructure.Repository
{
    public class NotificationRepository(WelfareWorkTrackerContext context) : INotificationRepository
    {
        private readonly WelfareWorkTrackerContext _context = context;
        public async Task<Notification?> CreateNotificationAsync(Notification notification)
        {
            var createdNotification = await _context.Notifications.AddAsync(notification);
            var rowsEffected = await _context.SaveChangesAsync();
            return rowsEffected > 0 ? createdNotification.Entity : null;
        }

        public async Task<bool> DeleteAllNotificationsAsync(int userId)
        {
            var notifications = await _context.Notifications
            .Where(n => n.ToUserId == userId && !n.IsDeleted)
            .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsDeleted = true;
                notification.DateUpdated = DateTime.UtcNow;
            }

            var rowsEffected = await _context.SaveChangesAsync();
            return rowsEffected > 0;
        }

        public async Task<bool> DeleteNotificationAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId)
            ?? throw new WelfareWorkTrackerException($"Failed to delete notification with id {notificationId}");

            notification.IsDeleted = true;
            notification.DateUpdated = DateTime.UtcNow;

            var rowsEffected = await _context.SaveChangesAsync();
            return rowsEffected > 0;
        }

        public async Task<List<Notification>> GetAllNotificationsByUserAsync(int userId)
        {
            var notifications = await _context.Notifications.Where(n => n.ToUserId == userId).ToListAsync();
            return notifications;
        }

        public async Task<Notification?> GetNotificationByIdAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            return notification;
        }

        public async Task<int> GetUnReadNotificationsCountAsync(int userId)
        {
            var count = await _context.Notifications.Where(n => n.ToUserId ==  userId && !n.IsRead).CountAsync();
            return count;
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            var notifications = await _context.Notifications
           .Where(n => n.ToUserId == userId && !n.IsDeleted)
           .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.DateUpdated = DateTime.UtcNow;
            }

            var rowsEffected = await _context.SaveChangesAsync();
            return rowsEffected > 0;
        }

        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId)
            ?? throw new WelfareWorkTrackerException($"Failed to delete notification with id {notificationId}");

            notification.IsRead = true;
            notification.DateUpdated = DateTime.UtcNow;

            var rowsEffected = await _context.SaveChangesAsync();
            return rowsEffected > 0;
        }
    }
}
