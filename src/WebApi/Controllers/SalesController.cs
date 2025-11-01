using Application.Features.Sales.Commands;
using Application.Features.Sales.Queries;
using Domain.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SalesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSalesDto request)
        {
            return Ok(await _mediator.Send(new CreateSalesCommand(request.CustomerId,
                request.TotalPrice,
                request.DiscountPercentage,
                request.TotalPaid,
                request.Remark,
                request.SalesItems,
                request.TaxPercentage)));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _mediator.Send(new GetSalesByIdQuery { Id = id }));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] Guid? customerId = null, [FromQuery] string? searchTerm = null)
        {
            return Ok(await _mediator.Send(new GetAllSalesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                CustomerId = customerId,
                SearchTerm = searchTerm
            }));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateSalesCommand command)
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
            return Ok(await _mediator.Send(new DeleteSalesCommand(id)));
        }

        [HttpPost("{salesId}/payments")]
        public async Task<IActionResult> AddPayment(Guid salesId, AddPaymentCommand command)
        {
            if (salesId != command.SalesId)
            {
                return BadRequest("Sales ID mismatch");
            }

            return Ok(await _mediator.Send(command));
        }

        [HttpGet("{salesId}/payments")]
        public async Task<IActionResult> GetPayments(Guid salesId)
        {
            return Ok(await _mediator.Send(new GetPaymentsBySalesQuery { SalesId = salesId }));
        }

        [HttpGet("{salesId}/pdf")]
        public async Task<IActionResult> GetSalesPdf(Guid salesId)
        {
            var response = await _mediator.Send(new GetSalesPdfQuery { SalesId = salesId });

            if (response.Succeeded && response.Data != null)
            {
                return File(response.Data, "application/pdf", $"Sales_{salesId.ToString().Substring(0, 8)}.pdf");
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
