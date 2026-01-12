using TodoApp.Application.Common;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Service
{
    public interface IUserService
    {
        Task<Result<User>> CreateUserAsync(User user);
        Task<Result<User?>> GetUserByIdAsync(int idUser);
        Task<Result<User?>> GetUserByUsernameAsync(string username);
        Task<Result<User?>> GetUserByEmailAsync(string email);
        Task<Result<IEnumerable<User>>> GetAllUsersAsync();
        Task<Result<User>> UpdateUserAsync(User user);
        Task<Result<bool>> DeleteUserAsync(int idUser);
    }

}
