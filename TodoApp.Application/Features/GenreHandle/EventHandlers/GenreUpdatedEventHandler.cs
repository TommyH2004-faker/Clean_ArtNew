using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;

namespace TodoApp.Application.Features.GenreHandle.EventHandlers
{
    public class GenreUpdatedEventHandler : INotificationHandler<GenreUpdatedEvent>
    {
        private readonly ILogger<GenreUpdatedEventHandler> _logger;

        public GenreUpdatedEventHandler(ILogger<GenreUpdatedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(GenreUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "🔄 Domain Event: Genre ID {GenreId} updated from '{OldName}' to '{NewName}'",
                notification.GenreId,
                notification.OldName,
                notification.NewName);

            await Task.CompletedTask;
        }
    }
}
