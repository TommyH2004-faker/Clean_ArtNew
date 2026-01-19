using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Repository;

namespace TodoApp.WebAPI.Controller
{
    /// <summary>
    /// Controller: Quản lý notifications cho admin dashboard
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(
            INotificationRepository notificationRepository,
            ILogger<NotificationsController> logger)
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
        }

        /// <summary>
        /// GET: api/notifications - Lấy tất cả notifications (có phân trang)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var notifications = await _notificationRepository.GetAllAsync(page, pageSize);
                var unreadCount = await _notificationRepository.CountUnreadAsync();

                return Ok(new
                {
                    success = true,
                    data = notifications,
                    unreadCount = unreadCount,
                    page = page,
                    pageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications");
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy danh sách thông báo" });
            }
        }

        /// <summary>
        /// GET: api/notifications/unread - Lấy notifications chưa đọc
        /// </summary>
        [HttpGet("unread")]
        public async Task<IActionResult> GetUnread()
        {
            try
            {
                var notifications = await _notificationRepository.GetUnreadAsync();
                return Ok(new
                {
                    success = true,
                    data = notifications,
                    count = notifications.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread notifications");
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy thông báo chưa đọc" });
            }
        }

        /// <summary>
        /// GET: api/notifications/count - Đếm notifications chưa đọc
        /// </summary>
        [HttpGet("count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                var count = await _notificationRepository.CountUnreadAsync();
                return Ok(new
                {
                    success = true,
                    count = count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting unread notifications");
                return StatusCode(500, new { success = false, message = "Lỗi khi đếm thông báo" });
            }
        }

        /// <summary>
        /// PUT: api/notifications/{id}/read - Đánh dấu notification đã đọc
        /// </summary>
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                var notification = await _notificationRepository.GetByIdAsync(id);
                if (notification == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy thông báo" });
                }

                notification.MarkAsRead();
                await _notificationRepository.UpdateAsync(notification);

                return Ok(new
                {
                    success = true,
                    message = "Đã đánh dấu đã đọc",
                    data = notification
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read");
                return StatusCode(500, new { success = false, message = "Lỗi khi cập nhật thông báo" });
            }
        }

        /// <summary>
        /// PUT: api/notifications/read-all - Đánh dấu tất cả đã đọc
        /// </summary>
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                await _notificationRepository.MarkAllAsReadAsync();
                return Ok(new
                {
                    success = true,
                    message = "Đã đánh dấu tất cả đã đọc"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read");
                return StatusCode(500, new { success = false, message = "Lỗi khi cập nhật thông báo" });
            }
        }
    }
}
