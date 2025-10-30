using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.Order.Commands
{
    public class AddPaymentCommandHandler : ICommandHandler<AddPaymentCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;

        public AddPaymentCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IResponse<bool>> Handle(AddPaymentCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Order? order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken);
            if (order == null)
            {
                throw new Exception("Order not found!");
            }

            // AddPayment will validate and update status automatically
            order.AddPayment(command.Amount, command.PaymentMethod, command.Remark);

            _orderRepository.Update(order);
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(true);
        }
    }
}
