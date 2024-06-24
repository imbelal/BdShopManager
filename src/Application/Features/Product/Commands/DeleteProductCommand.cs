using Common.RequestWrapper;

namespace Application.Features.Product.Commands
{
    public class DeleteProductCommand : ICommand<Guid>
    {
        public Guid ProductId { get; set; }

        public DeleteProductCommand(Guid productId)
        {
            ProductId = productId;
        }
    }
}
