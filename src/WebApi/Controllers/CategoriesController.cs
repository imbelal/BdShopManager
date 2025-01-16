using Application.Features.Category.Commands;
using Application.Features.Category.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateCategoryCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(DeleteCategoryCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var results = await _mediator.Send(new GetAllCategoriesQuery());

            return Ok(results);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetCategoryByIdQuery(id));

            return Ok(result);
        }
    }
}
