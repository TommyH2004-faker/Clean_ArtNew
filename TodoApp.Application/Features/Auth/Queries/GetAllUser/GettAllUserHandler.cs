using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs.UserDTOs;
using TodoApp.Application.Features.Auth.Queries.GetAllUser;
using TodoApp.Application.Repository;


public class GetAllUserHandle : IRequestHandler<GetAllUserQuery, Result<List<UserResponseDTO>>>
{
    private readonly IUserRepository _userRepository;
    public GetAllUserHandle(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<Result<List<UserResponseDTO>>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllUsersAsync();

        var userDTOs = users.Select(user => new UserResponseDTO
        {
            IdUser = user.IdUser,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.Username,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            DeliveryAddress = user.DeliveryAddress,
            Password = user.PasswordHash
        }).ToList();
        return Result<List<UserResponseDTO>>.Success(userDTOs);
    }
}