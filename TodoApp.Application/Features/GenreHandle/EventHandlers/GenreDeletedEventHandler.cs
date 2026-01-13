using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;

namespace TodoApp.Application.Features.GenreHandle.EventHandlers
{
    public class GenreDeletedEventHandler : INotificationHandler<GenreDeletedEvent>
    {
        private readonly ILogger<GenreDeletedEventHandler> _logger;

        public GenreDeletedEventHandler(ILogger<GenreDeletedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(GenreDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogWarning(
                "❌ Domain Event: Genre '{GenreName}' (ID: {GenreId}) was deleted",
                notification.GenreName,
                notification.GenreId);

            await Task.CompletedTask;
        }
    }
}
