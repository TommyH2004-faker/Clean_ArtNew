using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Features.Auth.Command;
using TodoApp.Application.Features.Auth.Command.Login;
using TodoApp.Application.Features.Auth.Command.Refresh;

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
   
}