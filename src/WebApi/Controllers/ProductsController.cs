using Application.Features.Product.Commands;
using Application.Features.Product.Queries;
using Domain.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateProductCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(Guid productId)
        {
            return Ok(await _mediator.Send(new DeleteProductCommand(productId)));
        }

        [HttpGet("GetProductWithPaging/{pageSize}/{pageNumber}")]
        public async Task<IActionResult> GetProducts(int pageSize, int pageNumber, [FromQuery] string searchTerm = null)
        {
            GetAllProductsQuery query = new(pageNumber, pageSize, searchTerm);
            return Ok(await _mediator.Send(query));
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetProductByIdQuery(id));

            return Ok(result);
        }

        [HttpPost("{productId}/photos")]
        public async Task<IActionResult> UploadPhoto(Guid productId, [FromForm] UploadProductPhotoDto dto)
        {
            var command = new UploadProductPhotoCommand(productId, dto.File, dto.IsPrimary, dto.DisplayOrder);
            return Ok(await _mediator.Send(command));
        }

        [HttpGet("{productId}/photos")]
        public async Task<IActionResult> GetProductPhotos(Guid productId)
        {
            var query = new GetProductPhotosQuery(productId);
            return Ok(await _mediator.Send(query));
        }

        [HttpDelete("photos/{photoId}")]
        public async Task<IActionResult> DeletePhoto(Guid photoId)
        {
            var command = new DeleteProductPhotoCommand(photoId);
            return Ok(await _mediator.Send(command));
        }

        [HttpPut("photos/{photoId}/set-primary")]
        public async Task<IActionResult> SetPrimaryPhoto(Guid photoId)
        {
            var command = new SetPrimaryProductPhotoCommand(photoId);
            return Ok(await _mediator.Send(command));
        }
    }
}
