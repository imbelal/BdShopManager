using Common.RequestWrapper;

namespace Application.Features.Product.Commands
{
    public class DeleteProductCommand : ICommand<Guid>
    {
        public Guid PostId { get; set; }

        public DeleteProductCommand(Guid postId)
        {
            PostId = postId;
        }
    }
}
