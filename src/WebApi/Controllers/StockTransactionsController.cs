using Application.Features.StockTransaction.Commands;
using Application.Features.StockTransaction.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StockTransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StockTransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllStockTransactionsQuery query)
        {
            return Ok(await _mediator.Send(query));
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProductId(Guid productId)
        {
            return Ok(await _mediator.Send(new GetStockTransactionsByProductIdQuery { ProductId = productId }));
        }

        [HttpPost("adjustment")]
        public async Task<IActionResult> CreateAdjustment(CreateStockAdjustmentCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
    }
}
