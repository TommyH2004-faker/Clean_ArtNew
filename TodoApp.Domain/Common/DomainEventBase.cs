namespace TodoApp.Domain.Common
{
    /// <summary>
    /// Base record cho Domain Events - Pure domain, no infrastructure dependencies
    /// </summary>
    public abstract record DomainEventBase : IDomainEvent
    {
        public DateTime OccurredOn { get; init; }

        protected DomainEventBase()
        {
            OccurredOn = DateTime.UtcNow;
        }
    }
}
