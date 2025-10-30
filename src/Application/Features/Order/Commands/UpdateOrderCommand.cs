using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.Order.Commands
{
    public class UpdateOrderCommand : ICommand<bool>
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TaxPercentage { get; set; } = 0;
        public string Remark { get; set; }
        public List<OrderDetailsDto> OrderDetails { get; set; }
    }
}
