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
                request.OrderDetails,
                request.TaxPercentage)));
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

        [HttpPost("{orderId}/payments")]
        public async Task<IActionResult> AddPayment(Guid orderId, AddPaymentCommand command)
        {
            if (orderId != command.OrderId)
            {
                return BadRequest("Order ID mismatch");
            }

            return Ok(await _mediator.Send(command));
        }

        [HttpGet("{orderId}/payments")]
        public async Task<IActionResult> GetPayments(Guid orderId)
        {
            return Ok(await _mediator.Send(new GetPaymentsByOrderQuery { OrderId = orderId }));
        }

        [HttpGet("{orderId}/pdf")]
        public async Task<IActionResult> GetOrderPdf(Guid orderId)
        {
            var response = await _mediator.Send(new GetOrderPdfQuery { OrderId = orderId });

            if (response.Succeeded && response.Data != null)
            {
                return File(response.Data, "application/pdf", $"Order_{orderId.ToString().Substring(0, 8)}.pdf");
            }

            return BadRequest("Failed to generate PDF");
        }

        [HttpGet("profit-summary")]
        public async Task<IActionResult> GetProfitSummary(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string groupBy = "day")
        {
            var query = new GetProfitSummaryQuery(startDate, endDate, groupBy);
            return Ok(await _mediator.Send(query));
        }
    }
}
