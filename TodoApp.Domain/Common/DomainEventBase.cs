namespace TodoApp.Domain.Common
{
    /// <summary>
    /// Base record cho Domain Events - Pure domain, no infrastructure dependencies
    /// Abstract class cung cấp implementation chung cho thuộc tính OccurredOn
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
