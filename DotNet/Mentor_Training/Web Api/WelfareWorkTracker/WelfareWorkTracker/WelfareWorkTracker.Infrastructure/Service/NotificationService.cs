using AutoMapper;
using System.Net;
using WelfareWorkTracker.Core.Contracts.Repository;
using WelfareWorkTracker.Core.Contracts.Service;
using WelfareWorkTracker.Core.Exceptions;
using WelfareWorkTracker.Core.Models;
using WelfareWorkTracker.Core.Vms;

namespace WelfareWorkTracker.Infrastructure.Service
{
    public class NotificationService(IMapper mapper,
                                        INotificationRepository notificationRepository) : INotificationService
    {
        private readonly IMapper _mapper = mapper;
        private readonly INotificationRepository _notificationRepository = notificationRepository;

        public async Task<Notification?> CreateNotificationAsync(CreateNotificationVm notificationVm)
        {
            var notification = _mapper.Map<Notification>(notificationVm);
            notification.DateCreated = DateTime.UtcNow;
            var createdNotification = await _notificationRepository.CreateNotificationAsync(notification);
            return createdNotification;
        }

        public async Task<bool> DeleteAllNotificationsAsync(int userId)
        {
            var notificationsDeleted = await _notificationRepository.DeleteAllNotificationsAsync(userId);
            return notificationsDeleted;
        }

        public async Task<bool> DeleteNotificationAsync(int userId, int notificationId)
        {
            var notificationsDeleted = await _notificationRepository.DeleteNotificationAsync(notificationId);
            return notificationsDeleted;
        }

        public async Task<List<Notification>> GetAllNotificationsByUserAsync(int userId)
        {
            var notifications = await _notificationRepository.GetAllNotificationsByUserAsync(userId);
            return notifications;
        }

        public async Task<Notification> GetNotificationByIdAsync(int notificationId)
        {
            var notification = await _notificationRepository.GetNotificationByIdAsync(notificationId)
                            ?? throw new WelfareWorkTrackerException("Notification not found.", (int)HttpStatusCode.NotFound);
            return notification;
        }

        public async Task<int> GetUnReadNotificationsCountAsync(int userId)
        {
            var count = await _notificationRepository.GetUnReadNotificationsCountAsync(userId);
            return count;
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            var notificationsRead = await _notificationRepository.MarkAllAsReadAsync(userId);
            return notificationsRead;
        }

        public async Task<bool> MarkAsReadAsync(int userId, int notificationId)
        {
            var notificationRead = await _notificationRepository.MarkAsReadAsync(notificationId);
            return notificationRead;
        }
    }

}
