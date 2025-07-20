using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.Product.Queries
{
    public class GetProductPhotosQuery : IQuery<List<ProductPhotoDto>>
    {
        public Guid ProductId { get; set; }

        public GetProductPhotosQuery(Guid productId)
        {
            ProductId = productId;
        }
    }
} 