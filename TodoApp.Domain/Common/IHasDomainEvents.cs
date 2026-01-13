namespace TodoApp.Domain.Common
{
    /// <summary>
    /// Interface cho Aggregate Roots có thể raise Domain Events
    /// </summary>
    public interface IHasDomainEvents
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
        void AddDomainEvent(IDomainEvent domainEvent);
        void RemoveDomainEvent(IDomainEvent domainEvent);
        void ClearDomainEvents();
    }
}
