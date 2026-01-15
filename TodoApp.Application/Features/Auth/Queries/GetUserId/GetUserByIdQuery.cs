using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs.UserDTOs;

namespace TodoApp.Application.Features.Auth.Queries.GetUserId;
public class GetUserByIdQuery(int IdUser) : IRequest<Result<UserResponseDTO>>
{
 public int IdUser { get; init; } = IdUser;  
}