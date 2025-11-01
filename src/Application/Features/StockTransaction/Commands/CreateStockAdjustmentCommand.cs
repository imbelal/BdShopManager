using Common.RequestWrapper;
using Domain.Enums;

namespace Application.Features.StockTransaction.Commands
{
    public class CreateStockAdjustmentCommand : ICommand<Guid>
    {
        public Guid ProductId { get; set; }
        public StockTransactionType Type { get; set; } // IN or OUT
        public int Quantity { get; set; }
        public string Reason { get; set; }
    }
}
