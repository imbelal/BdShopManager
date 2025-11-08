using Application.Features.Expense.Commands;
using Application.Features.Expense.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExpensesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateExpenseCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateExpenseCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(DeleteExpenseCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPost("Approve")]
        public async Task<IActionResult> Approve(ApproveExpenseCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPost("Reject")]
        public async Task<IActionResult> Reject(RejectExpenseCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPost("MarkAsPaid")]
        public async Task<IActionResult> MarkAsPaid(MarkExpenseAsPaidCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllExpensesQuery query)
        {
            var results = await _mediator.Send(query);
            return Ok(results);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetExpenseByIdQuery(id));
            return Ok(result);
        }

        [HttpGet("pdf")]
        public async Task<IActionResult> GetExpensesPdf([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var response = await _mediator.Send(new GetExpensesPdfQuery
            {
                StartDate = startDate,
                EndDate = endDate
            });

            if (response.Succeeded && response.Data != null)
            {
                var fileName = "Expenses_";
                if (startDate.HasValue && endDate.HasValue)
                {
                    fileName += $"{startDate.Value:yyyyMMdd}_to_{endDate.Value:yyyyMMdd}";
                }
                else if (startDate.HasValue)
                {
                    fileName += $"from_{startDate.Value:yyyyMMdd}";
                }
                else if (endDate.HasValue)
                {
                    fileName += $"until_{endDate.Value:yyyyMMdd}";
                }
                else
                {
                    fileName += $"all_{DateTime.Now:yyyyMMdd}";
                }
                fileName += ".pdf";

                return File(response.Data, "application/pdf", fileName);
            }

            return BadRequest("Failed to generate PDF");
        }
    }
}