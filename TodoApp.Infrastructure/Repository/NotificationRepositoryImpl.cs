using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Repository;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence;

namespace TodoApp.Infrastructure.Repository
{
    /// <summary>
    /// Implementation: NotificationRepository
    /// </summary>
    public class NotificationRepositoryImpl : INotificationRepository
    {
        private readonly TodoAppDbContext _context;

        public NotificationRepositoryImpl(TodoAppDbContext context)
        {
            _context = context;
        }

        public async Task<Notification> AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<List<Notification>> GetAllAsync(int page = 1, int pageSize = 50)
        {
            return await _context.Notifications
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetUnreadAsync()
        {
            return await _context.Notifications
                .Where(n => !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> CountUnreadAsync()
        {
            return await _context.Notifications
                .CountAsync(n => !n.IsRead);
        }

        public async Task<Notification?> GetByIdAsync(int idNotification)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(n => n.IdNotification == idNotification);
        }

        public async Task UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAllAsReadAsync()
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.MarkAsRead();
            }

            await _context.SaveChangesAsync();
        }
    }
}
