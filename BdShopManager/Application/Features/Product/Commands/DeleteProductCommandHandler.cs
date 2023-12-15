using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.Product.Commands
{
    public class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand, Guid>
    {
        private readonly IPostRepository _postRepository;

        public DeleteProductCommandHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }
        public async Task<IResponse<Guid>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.Product product = await _postRepository.GetByIdAsync(request.PostId) ?? throw new KeyNotFoundException("User not found!!");
            _postRepository.Remove(product);
            await _postRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(product.Id);
        }
    }
}
