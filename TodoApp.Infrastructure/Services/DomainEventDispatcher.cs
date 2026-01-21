using System.Collections.Concurrent;
using System.Reflection;
using MediatR;
using TodoApp.Application.Events;
using TodoApp.Domain.Common;

namespace TodoApp.Infrastructure.Services
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IMediator _mediator;
        
        private static readonly ConcurrentDictionary<Type, Type?> _eventWrapperCache = new();
        
        private static readonly ConcurrentDictionary<Type, ConstructorInfo?> _constructorCache = new();

        public DomainEventDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }
         // Auto-discovery: Tự động tìm wrapper cho domain event
        public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            // 1. Tìm wrapper phù hợp ( UserRegistered → UserRegisteredEvent)
            var notification = CreateNotification(domainEvent);
            
            if (notification != null)
            {
                  // 2. Publish qua MediatR
                await _mediator.Publish(notification, cancellationToken);
            }
        }

        public async Task DispatchAllAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
        {
            foreach (var domainEvent in domainEvents)
            {
                await DispatchAsync(domainEvent, cancellationToken);
            }
        }

        private INotification? CreateNotification(IDomainEvent domainEvent)
        {
            var domainEventType = domainEvent.GetType();
            
        // Dùng Reflection tìm wrapper
        // UserRegistered → tìm class implement IDomainEventWrapper<UserRegistered>
            var wrapperType = _eventWrapperCache.GetOrAdd(domainEventType, FindWrapperType);
            
            if (wrapperType == null)
            {
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
