using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.Purchase.Commands
{
    public class UpdatePurchaseCommand : ICommand<bool>
    {
        public Guid Id { get; set; }
        public Guid SupplierId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Remark { get; set; }
        public List<CreatePurchaseItemDto> PurchaseItems { get; set; } = new();
    }
}
