using Common.RequestWrapper;

namespace Application.Features.Inventory.Commands
{
    public class DeleteInventoryCommand : ICommand<bool>
    {
        public Guid Id { get; set; }
    }
}
