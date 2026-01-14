using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Repository;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence;

namespace TodoApp.Infrastructure.Repository
{
    public class OrderRepositoryImpl : IOrderRepository
    {
        private readonly TodoAppDbContext _context;

        public OrderRepositoryImpl(TodoAppDbContext context)
        {
            _context = context;
        }

        public async Task<Orders?> GetByIdAsync(int idOrder)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.IdOrder == idOrder);
        }

        public async Task<Orders?> GetByIdWithDetailsAsync(int idOrder)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Book)
                .Include(o => o.User)
                .Include(o => o.Payment)
                .Include(o => o.Delivery)
                .FirstOrDefaultAsync(o => o.IdOrder == idOrder);
        }

        public async Task<List<Orders>> GetAllWithDetailsAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Book)
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Orders>> GetByUserIdAsync(int idUser)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Book)
                .Where(o => o.IdUser == idUser)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Orders order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
