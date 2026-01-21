using System.Text.Json;
using TodoApp.Application.Repository;
using TodoApp.Application.Service;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Services
{
    /// <summary>
    /// Implementation: Lưu notification vào DB (không push SignalR ở tầng này)
    /// </summary>
    public class SignalRNotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public SignalRNotificationService(
            INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task SendOrderNotificationAsync(string title, string message, object? metadata = null)
        {
            await SendNotificationAsync(NotificationType.Order, title, message, metadata);
        }

        public async Task SendUserNotificationAsync(string title, string message, object? metadata = null)
        {
            await SendNotificationAsync(NotificationType.User, title, message, metadata);
        }

        public async Task SendSystemNotificationAsync(string title, string message, object? metadata = null)
        {
            await SendNotificationAsync(NotificationType.System, title, message, metadata);
        }

        public async Task SendNotificationAsync(NotificationType type, string title, string message, object? metadata = null)
        {
            // 1. Serialize metadata thành JSON
            var metadataJson = metadata != null ? JsonSerializer.Serialize(metadata) : null;

            // 2. Tạo Notification entity
            var notification = Notification.Create(type, title, message, metadataJson);

            // 3. Lưu vào database
            var savedNotification = await _notificationRepository.AddAsync(notification);

            Console.WriteLine($"[ NOTIFICATION] {type} - {title} (ID: {savedNotification.IdNotification})");
        }
    }
}

