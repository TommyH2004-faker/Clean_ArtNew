using TodoApp.Application.Common;
using TodoApp.Application.Repository;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<User>> CreateUserAsync(User user)
        {
            var existingUserByUsername = await _userRepository.GetUserByUsernameAsync(user.Username);
            if (existingUserByUsername != null)
            {
                return Result<User>.Failure(ErrorType.Conflict, "Username already exists.");
            }

            var existingUserByEmail = await _userRepository.GetUserByEmailAsync(user.Email);
            if (existingUserByEmail != null)
            {
                return Result<User>.Failure(ErrorType.Conflict, "Email already exists.");
            }

            await _userRepository.AddUserAsync(user);
            return Result<User>.Success(user);
        }

        public async Task<Result<bool>> DeleteUserAsync(int idUser)
        {
            var user = await _userRepository.GetUserByIdAsync(idUser);
            if (user == null)
            {
                return Result<bool>.Failure(ErrorType.NotFound, "User not found.");
            }

            await _userRepository.DeleteUserAsync(user);
            return Result<bool>.Success(true);
        }

        public async Task<Result<IEnumerable<User>>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Result<IEnumerable<User>>.Success(users);
        }

        public async Task<Result<User?>> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return Result<User?>.Failure(ErrorType.NotFound, "User not found.");
            }
            return Result<User?>.Success(user);
        }

        public async Task<Result<User?>> GetUserByIdAsync(int idUser)
        {
            var user = await _userRepository.GetUserByIdAsync(idUser);
            if (user == null)
            {
                return Result<User?>.Failure(ErrorType.NotFound, "User not found.");
            }
            return Result<User?>.Success(user);
        }

        public async Task<Result<User?>> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null)
            {
                return Result<User?>.Failure(ErrorType.NotFound, "User not found.");
            }
            return Result<User?>.Success(user);
        }

        public async Task<Result<User>> UpdateUserAsync(User user)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(user.IdUser);
            if (existingUser == null)
            {
                return Result<User>.Failure(ErrorType.NotFound, "User not found.");
            }

            await _userRepository.UpdateUserAsync(user);
            return Result<User>.Success(user);
        }
    }
}