using Application.Features.Purchase.Commands;
using Application.Features.Purchase.Queries;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PurchasesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePurchaseCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] Guid? supplierId = null,
            [FromQuery] PurchaseStatus? status = null,
            [FromQuery] Guid? productId = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var query = new GetPurchasesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SupplierId = supplierId,
                Status = status,
                ProductId = productId,
                SearchTerm = searchTerm,
                StartDate = startDate,
                EndDate = endDate
            };
            return Ok(await _mediator.Send(query));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdatePurchaseCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID mismatch");
            }

            return Ok(await _mediator.Send(command));
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            return Ok(await _mediator.Send(new CancelPurchaseCommand { Id = id }));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _mediator.Send(new DeletePurchaseCommand { Id = id }));
        }
    }
}
