using Application.Features.SalesReturn.Commands;
using Application.Features.SalesReturn.Queries;
using Domain.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SalesReturnsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SalesReturnsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSalesReturnDto request)
        {
            return Ok(await _mediator.Send(new CreateSalesReturnCommand(
                request.SalesId,
                request.TotalRefundAmount,
                request.Remark,
                request.SalesReturnItems)));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _mediator.Send(new GetSalesReturnByIdQuery { Id = id }));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] Guid? salesId = null,
            [FromQuery] string? searchTerm = null)
        {
            return Ok(await _mediator.Send(new GetAllSalesReturnsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SalesId = salesId,
                SearchTerm = searchTerm
            }));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _mediator.Send(new DeleteSalesReturnCommand(id)));
        }
    }
}
