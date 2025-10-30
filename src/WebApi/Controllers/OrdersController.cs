using Application.Features.Order.Commands;
using Application.Features.Order.Queries;
using Domain.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDto request)
        {
            return Ok(await _mediator.Send(new CreateOrderCommand(request.CustomerId,
                request.TotalPrice,
                request.TotalPaid,
                request.Remark,
                request.OrderDetails)));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _mediator.Send(new GetOrderByIdQuery { Id = id }));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] Guid? customerId = null, [FromQuery] string? searchTerm = null)
        {
            return Ok(await _mediator.Send(new GetAllOrdersQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                CustomerId = customerId,
                SearchTerm = searchTerm
            }));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateOrderCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID mismatch");
            }

            return Ok(await _mediator.Send(command));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _mediator.Send(new DeleteOrderCommand(id)));
        }
    }
}
