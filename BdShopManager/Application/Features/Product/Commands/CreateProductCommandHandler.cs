using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.Product.Commands
{
    public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, Guid>
    {
        private readonly IPostRepository _postRepository;

        public CreateProductCommandHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<IResponse<Guid>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Product product = new(command.Title, command.Description, command.CategoryId, command.StockQuantity, command.Unit, command.TagIds);
            _postRepository.Add(product);
            await _postRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(product.Id);
        }
    }
}
