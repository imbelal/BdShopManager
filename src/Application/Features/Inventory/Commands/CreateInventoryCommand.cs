using Common.RequestWrapper;

namespace Application.Features.Inventory.Commands
{
    public class CreateInventoryCommand : ICommand<Guid>
    {
        public Guid ProductId { get; set; }
        public Guid SupplierId { get; set; }
        public int Quantity { get; set; }
        public decimal CostPerUnit { get; set; }
        public string Remark { get; set; }
    }
}
