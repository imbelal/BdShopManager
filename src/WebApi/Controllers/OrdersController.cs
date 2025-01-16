using Application.Features.Order.Commands;
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

        [HttpDelete]
        public async Task<IActionResult> Delete(DeleteOrderCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
    }
}
