using Application.Features.Product.Commands;
using Application.Features.Product.Queries;
using Domain.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/v1/[controller]")]
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
        public async Task<IActionResult> GetProducts(
            int pageSize = 10,
            int pageNumber = 1,
            [FromQuery] string searchTerm = null,
            [FromQuery] Guid? categoryId = null,
            [FromQuery] string sortBy = null,
            [FromQuery] string sortOrder = null,
            [FromQuery] int? unit = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] bool? inStock = null)
        {
            GetAllProductsQuery query = new(
                pageNumber,
                pageSize,
                searchTerm,
                categoryId,
                sortBy,
                sortOrder,
                unit,
                minPrice,
                maxPrice,
                inStock);
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

        [HttpGet("select-list")]
        public async Task<IActionResult> GetProductIdTitleList()
        {
            var result = await _mediator.Send(new GetProductIdTitleListQuery());
            return Ok(result);
        }

        [HttpGet("profitability")]
        public async Task<IActionResult> GetProductProfitability(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] decimal? minimumProfitMargin = null)
        {
            var query = new GetProductProfitabilityQuery(pageNumber, pageSize, startDate, endDate, minimumProfitMargin);
            return Ok(await _mediator.Send(query));
        }

        [HttpGet("low-profit")]
        public async Task<IActionResult> GetLowProfitProducts(
            [FromQuery] decimal maxProfitMargin = 10.0m,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetLowProfitProductsQuery(maxProfitMargin, pageNumber, pageSize);
            return Ok(await _mediator.Send(query));
        }
    }
}
