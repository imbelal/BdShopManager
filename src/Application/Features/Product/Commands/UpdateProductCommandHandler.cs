using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Product.Commands
{
    public class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand, Guid>
    {
        private readonly IProductRepository _productRepository;
        private readonly IReadOnlyApplicationDbContext _applicationDbContext;

        public UpdateProductCommandHandler(IReadOnlyApplicationDbContext applicationDbContext, IProductRepository productRepository)
        {
            _applicationDbContext = applicationDbContext;
            _productRepository = productRepository;
        }

        public async Task<IResponse<Guid>> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Product product = await _applicationDbContext.Products.Include(p => p.ProductTags)
                .FirstOrDefaultAsync(p => p.Id == command.ProductId, cancellationToken);

            if (product == null)
                return Response.Fail<Guid>("Product not found");

            product.Update(command.Title, command.Description, command.CategoryId, command.Unit, command.SellingPrice);

            _productRepository.Update(product);
            await _productRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(product.Id);
        }
    }
}
