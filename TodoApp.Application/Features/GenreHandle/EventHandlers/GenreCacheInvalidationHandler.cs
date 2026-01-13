using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;

namespace TodoApp.Application.Features.GenreHandle.EventHandlers
{
    /// <summary>
    /// Handler chịu trách nhiệm clear cache khi Genre thay đổi.
    /// 
    /// Khi Genre được tạo/sửa/xóa, cache cũ sẽ không còn chính xác.
    /// Handler này đảm bảo lần request tiếp theo sẽ fetch data mới từ DB.
    /// 
    /// Side Effect: Cache Invalidation
    /// </summary>
    public class GenreCacheInvalidationHandler :
        INotificationHandler<GenreCreatedEvent>,
        INotificationHandler<GenreUpdatedEvent>,
        INotificationHandler<GenreDeletedEvent>
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<GenreCacheInvalidationHandler> _logger;

        // Cache keys - có thể move ra constants class
        private const string ALL_GENRES_CACHE_KEY = "genres:all";
        private const string GENRE_BY_ID_PREFIX = "genres:id:";

        public GenreCacheInvalidationHandler(
            IMemoryCache cache,
            ILogger<GenreCacheInvalidationHandler> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Khi Genre mới được tạo → Clear cache danh sách
        /// </summary>
        public Task Handle(GenreCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "🗑️ [CACHE] Invalidating genres cache after CREATE. GenreId: {GenreId}, Name: {GenreName}",
                notification.GenreId,
                notification.GenreName);

            // Clear cache danh sách tất cả genres
            _cache.Remove(ALL_GENRES_CACHE_KEY);

            _logger.LogDebug("🗑️ [CACHE] Removed key: {CacheKey}", ALL_GENRES_CACHE_KEY);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Khi Genre được cập nhật → Clear cache danh sách + cache của genre đó
        /// </summary>
        public Task Handle(GenreUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "🗑️ [CACHE] Invalidating genres cache after UPDATE. GenreId: {GenreId}, OldName: {OldName} → NewName: {NewName}",
                notification.GenreId,
                notification.OldName,
                notification.NewName);

            // Clear cache danh sách
            _cache.Remove(ALL_GENRES_CACHE_KEY);

            // Clear cache của genre cụ thể
            var genreKey = $"{GENRE_BY_ID_PREFIX}{notification.GenreId}";
            _cache.Remove(genreKey);

            _logger.LogDebug("🗑️ [CACHE] Removed keys: {AllKey}, {GenreKey}", ALL_GENRES_CACHE_KEY, genreKey);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Khi Genre bị xóa → Clear cache danh sách + cache của genre đó
        /// </summary>
        public Task Handle(GenreDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "🗑️ [CACHE] Invalidating genres cache after DELETE. GenreId: {GenreId}, Name: {GenreName}",
                notification.GenreId,
                notification.GenreName);

            // Clear cache danh sách
            _cache.Remove(ALL_GENRES_CACHE_KEY);

            // Clear cache của genre cụ thể
            var genreKey = $"{GENRE_BY_ID_PREFIX}{notification.GenreId}";
            _cache.Remove(genreKey);

            _logger.LogDebug("🗑️ [CACHE] Removed keys: {AllKey}, {GenreKey}", ALL_GENRES_CACHE_KEY, genreKey);

            return Task.CompletedTask;
        }
    }
}
