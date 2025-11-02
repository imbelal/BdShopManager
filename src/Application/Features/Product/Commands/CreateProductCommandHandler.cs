using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Product.Commands
{
    public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, Guid>
    {
        private readonly IProductRepository _productRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public CreateProductCommandHandler(IProductRepository productRepository, IReadOnlyApplicationDbContext context)
        {
            _productRepository = productRepository;
            _context = context;
        }

        public async Task<IResponse<Guid>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id.Equals(command.CategoryId), cancellationToken);
            if (category == null)
                return Response.Fail<Guid>("Category not found");

            Domain.Entities.Product product = new(command.Title, command.Description, command.Size, command.Color, command.CategoryId, command.Unit, command.SellingPrice, command.TagIds);
            _productRepository.Add(product);
            await _productRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(product.Id);
        }
    }
}
