using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs.UserDTOs;

namespace TodoApp.Application.Features.Auth.Queries.GetAllUser;
public  record GetAllUserQuery : IRequest<Result<List<UserResponseDTO>>>
{
    // Có thể thêm filter/pagination parameters sau
    // thêm check email hoặc username nếu cần
}