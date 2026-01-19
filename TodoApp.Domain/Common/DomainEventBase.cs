namespace TodoApp.Domain.Common
{
    /// <summary>
    /// // Base class cho tất cả events - tự động ghi thời gian
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
