using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs.UserDTOs;
using TodoApp.Application.Repository;

namespace TodoApp.Application.Features.Auth.Queries.GetUserId;

public class GetUserByIdHandle : IRequestHandler<GetUserByIdQuery, Result<UserResponseDTO>>
{
    private readonly IUserRepository _userRepository;
    public GetUserByIdHandle(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserResponseDTO>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.IdUser);
        if (user == null)
        {
            return Result<UserResponseDTO>.Failure(ErrorType.NotFound, "User not found");
        }
        var userDTO = new UserResponseDTO
        {
            IdUser = user.IdUser,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.Username,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            DeliveryAddress = user.DeliveryAddress,
            Password = user.PasswordHash
        };
        return Result<UserResponseDTO>.Success(userDTO);
    }
}