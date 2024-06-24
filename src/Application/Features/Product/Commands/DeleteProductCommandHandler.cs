using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.Product.Commands
{
    public class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand, Guid>
    {
        private readonly IProductRepository _productRepository;

        public DeleteProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<IResponse<Guid>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.Product product = await _productRepository.GetByIdAsync(request.ProductId) ?? throw new KeyNotFoundException("User not found!!");
            _productRepository.Remove(product);
            await _productRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(product.Id);
        }
    }
}
