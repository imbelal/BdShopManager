using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Product.Commands
{
    public class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand, Guid>
    {
        private readonly IPostRepository _postRepository;
        private readonly IReadOnlyApplicationDbContext _applicationDbContext;

        public UpdateProductCommandHandler(IReadOnlyApplicationDbContext applicationDbContext, IPostRepository postRepository)
        {
            _applicationDbContext = applicationDbContext;
            _postRepository = postRepository;
        }

        public async Task<IResponse<Guid>> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Product product = await _applicationDbContext.Products.Include(p => p.PostTags)
                .FirstOrDefaultAsync(p => p.Id == command.PostId, cancellationToken) ?? throw new KeyNotFoundException("User not found!!");

            product.Title = command.Title;
            product.Description = command.Description;
            product.CategoryId = command.CategoryId;
            product.StockQuantity = command.StockQuantity;
            product.Unit = command.Unit;
            product.UpdateTags(command.TagIds);

            _postRepository.Update(product);
            await _postRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(product.Id);
        }
    }
}
