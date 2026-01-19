namespace TodoApp.Domain.Common
{
    /// <summary>
   /// Interface đánh dấu một event trong hệ thống
    /// </summary>
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
