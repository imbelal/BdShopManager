using Application.Features.UserRole.Commands;
using Application.Features.UserRole.Queires;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserRoleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserRoleCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateUserRoleCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(DeleteUserRoleCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var results = await _mediator.Send(new GetAllUserRolesQuery());

            return Ok(results);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var results = await _mediator.Send(new GetUserRoleByIdQuery(id));

            return Ok(results);
        }
    }
}
