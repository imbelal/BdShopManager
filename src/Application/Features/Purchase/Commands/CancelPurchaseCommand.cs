using Common.RequestWrapper;

namespace Application.Features.Purchase.Commands
{
    public class CancelPurchaseCommand : ICommand<bool>
    {
        public Guid Id { get; set; }
    }
}