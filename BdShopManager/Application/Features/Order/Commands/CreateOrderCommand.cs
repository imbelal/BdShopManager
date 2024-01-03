using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.Order.Commands
{
    public class CreateOrderCommand : ICommand<Guid>
    {
        public Guid CustomerId { get; private set; }
        public decimal TotalPrice { get; private set; }
        public decimal TotalPaid { get; private set; }
        public string Remark { get; private set; }
        public List<OrderDetailsDto> OrderDetails { get; private set; }

        public CreateOrderCommand(Guid customerId, decimal totalPrice, decimal totalPaid, string remark, List<OrderDetailsDto> orderDetailDtos)
        {
            CustomerId = customerId;
            TotalPrice = totalPrice;
            TotalPaid = totalPaid;
            Remark = remark;
            OrderDetails = orderDetailDtos;
        }
    }
}
