using System.Collections.Concurrent;
using System.Reflection;
using MediatR;
using TodoApp.Application.Events;
using TodoApp.Domain.Common;

namespace TodoApp.Infrastructure.Services
{
    /// <summary>
    /// Service tự động convert Domain Events → MediatR Notifications.
    /// Sử dụng reflection để auto-discover event wrappers.
    /// 
    /// Convention: 
    /// - Domain Event: GenreEvents.GenreCreated (Domain Layer)
    /// - Wrapper: GenreCreatedEvent : IDomainEventWrapper<GenreCreated> (Application Layer)
    /// 
    /// Khi thêm event mới, chỉ cần tạo wrapper implement IDomainEventWrapper<T>,
    /// DomainEventDispatcher sẽ tự động tìm và map.
    /// </summary>
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IMediator _mediator;
        
        // Cache mapping: DomainEventType → WrapperType để tránh reflection mỗi lần
        private static readonly ConcurrentDictionary<Type, Type?> _eventWrapperCache = new();
        
        // Cache constructor info để tạo instance nhanh hơn
        private static readonly ConcurrentDictionary<Type, ConstructorInfo?> _constructorCache = new();

        public DomainEventDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Dispatch một domain event bằng cách tự động tìm wrapper phù hợp
        /// </summary>
        public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            var notification = CreateNotification(domainEvent);
            
            if (notification != null)
            {
                await _mediator.Publish(notification, cancellationToken);
            }
        }

        /// <summary>
        /// Dispatch nhiều domain events
        /// </summary>
        public async Task DispatchAllAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
        {
            foreach (var domainEvent in domainEvents)
            {
                await DispatchAsync(domainEvent, cancellationToken);
            }
        }

        /// <summary>
        /// Tạo INotification wrapper từ Domain Event
        /// </summary>
        private INotification? CreateNotification(IDomainEvent domainEvent)
        {
            var domainEventType = domainEvent.GetType();
            
            // Tìm wrapper type từ cache hoặc qua reflection
            var wrapperType = _eventWrapperCache.GetOrAdd(domainEventType, FindWrapperType);
            
            if (wrapperType == null)
            {
                // Không tìm thấy wrapper → log warning (có thể thêm ILogger)
                return null;
            }

            // Tạo instance của wrapper
            var constructor = _constructorCache.GetOrAdd(wrapperType, t => 
                t.GetConstructor(new[] { domainEventType }));
            
            if (constructor == null)
            {
                return null;
            }

            return constructor.Invoke(new object[] { domainEvent }) as INotification;
        }

        /// <summary>
        /// Tìm wrapper type cho một Domain Event type cụ thể.
        /// Scan tất cả types implement IDomainEventWrapper<TDomainEvent>
        /// </summary>
        private static Type? FindWrapperType(Type domainEventType)
        {
            // Interface cần tìm: IDomainEventWrapper<TDomainEvent>
            var targetInterface = typeof(IDomainEventWrapper<>).MakeGenericType(domainEventType);
            
            // Scan Application assembly để tìm wrapper
            var applicationAssembly = typeof(IDomainEventWrapper).Assembly;
            
            var wrapperType = applicationAssembly.GetTypes()
                .FirstOrDefault(t => 
                    !t.IsAbstract && 
                    !t.IsInterface && 
                    targetInterface.IsAssignableFrom(t));

            return wrapperType;
        }
    }

    /// <summary>
    /// Interface cho DomainEventDispatcher - để DI và testing
    /// </summary>
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
        Task DispatchAllAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
    }
}
