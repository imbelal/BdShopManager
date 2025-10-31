using Common.RequestWrapper;

namespace Application.Features.Purchase.Commands
{
    public class DeletePurchaseCommand : ICommand<bool>
    {
        public Guid Id { get; set; }
    }
}
