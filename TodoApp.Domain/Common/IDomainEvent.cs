namespace TodoApp.Domain.Common
{
    /// <summary>
    /// Marker interface cho Domain Events - Pure domain concept
    /// Không phụ thuộc vào bất kỳ infrastructure nào (MediatR, etc.)
    /// Interface định nghĩa "Sự kiện là gì?"
    /// </summary>
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
