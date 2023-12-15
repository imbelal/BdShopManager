using Application.Features.User.Commands;
using Application.Features.User.Commands.Auth;
using Application.Features.User.Queries;
using Application.Services.Auth.Dtos;
using Domain.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateUserRequestDto request)
        {
            return Ok(await _mediator.Send(new CreateUserCommand(request)));
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateUserInfo(UpdateUserInfoCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPut("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(UpdatePasswordCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPut("ResetPassword")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var results = await _mediator.Send(new GetAllUsersQuery());

            return Ok(results);
        }

        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery(id));

            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(LoginUserRequest loginUserRequest, CancellationToken cancellationToken = new())
        {
            return Ok(await _mediator.Send(new LoginUserCommand(loginUserRequest), cancellationToken));
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<AuthenticateResponse>> RefreshToken(RefreshTokenRequest refreshRequest, CancellationToken cancellationToken = new())
        {
            return Ok(await _mediator.Send(new RefreshTokenCommand(refreshRequest), cancellationToken));
        }
    }
}
