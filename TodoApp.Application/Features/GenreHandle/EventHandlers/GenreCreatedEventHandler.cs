using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;

namespace TodoApp.Application.Features.GenreHandle.EventHandlers
{
    /// <summary>
    /// Handler xử lý GenreCreated Domain Event
    /// Side effects: Log activity, Clear cache, Send notifications
    /// </summary>
    public class GenreCreatedEventHandler : INotificationHandler<GenreCreatedEvent>
    {
        private readonly ILogger<GenreCreatedEventHandler> _logger;

        public GenreCreatedEventHandler(ILogger<GenreCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(GenreCreatedEvent notification, CancellationToken cancellationToken)
        {
            // Side effect: Log activity
            _logger.LogInformation(
                "✅ Domain Event: Genre '{GenreName}' (ID: {GenreId}) was created at {OccurredOn}",
                notification.GenreName,
                notification.GenreId,
                notification.OccurredOn);

            await Task.CompletedTask;
        }
    }
}

