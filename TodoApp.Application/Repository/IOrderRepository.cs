using TodoApp.Domain.Entities;

namespace TodoApp.Application.Repository
{
    public interface IOrderRepository
    {
        Task<Orders?> GetByIdAsync(int idOrder);
        Task<Orders?> GetByIdWithDetailsAsync(int idOrder);
        Task<List<Orders>> GetAllWithDetailsAsync();
        Task<List<Orders>> GetByUserIdAsync(int idUser);
        Task AddAsync(Orders order);
        Task SaveChangesAsync();
    }
}
