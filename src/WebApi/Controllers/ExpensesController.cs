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
    }
}