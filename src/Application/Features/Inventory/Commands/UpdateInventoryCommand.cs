using Common.RequestWrapper;

namespace Application.Features.Inventory.Commands
{
    public class UpdateInventoryCommand : ICommand<bool>
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid SupplierId { get; set; }
        public int Quantity { get; set; }
        public decimal CostPerUnit { get; set; }
        public string Remark { get; set; }
    }
}
