using MediatR;
using TodoApp.Domain.Common;

namespace TodoApp.Application.Events
{
    /// <summary>
    /// Interface marker cho tất cả Domain Event wrappers.
    /// Cho phép auto-discovery và generic dispatch.
    /// </summary>
    public interface IDomainEventWrapper : INotification
    {
        /// <summary>
        /// Domain Event gốc từ Domain Layer
        /// </summary>
        IDomainEvent DomainEvent { get; }
    }

    /// <summary>
    /// Generic wrapper interface cho type-safe conversion
    /// </summary>
    /// <typeparam name="TDomainEvent">Loại Domain Event cụ thể</typeparam>
    public interface IDomainEventWrapper<TDomainEvent> : IDomainEventWrapper
        where TDomainEvent : IDomainEvent
    {
        /// <summary>
        /// Domain Event với type cụ thể
        /// </summary>
        new TDomainEvent DomainEvent { get; }
    }
}
