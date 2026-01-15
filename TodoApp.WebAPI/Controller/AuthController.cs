using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Features.Auth.Command;
using TodoApp.Application.Features.Auth.Command.Activate;
using TodoApp.Application.Features.Auth.Command.Login;
using TodoApp.Application.Features.Auth.Command.Refresh;
using TodoApp.Application.Features.Auth.Command.Register;
using TodoApp.Application.Features.Auth.Queries.GetAllUser;
using TodoApp.Application.Features.Auth.Queries.GetUserId;

namespace TodoApp.WebAPI.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    /// <summary>
    /// Kích hoạt tài khoản qua activation code
    /// </summary>
    [HttpPost("activate")]
    public async Task<IActionResult> ActivateUser([FromBody] ActivateUserCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                TodoApp.Application.Common.ErrorType.NotFound => NotFound(new { message = result.ErrorMessage }),
                TodoApp.Application.Common.ErrorType.Validation => BadRequest(new { message = result.ErrorMessage }),
                _ => StatusCode(500, new { message = result.ErrorMessage })
            };
        }

        return Ok(new { message = result.Data });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var query = new GetAllUserQuery();
        var response = await _mediator.Send(query);
        return Ok(response);
    }
    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var query = new GetUserByIdQuery(id);
        var response = await _mediator.Send(query);
        return Ok(response);
    }
   
   
}