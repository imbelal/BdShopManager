using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;

namespace Application.Features.Order.Commands
{
    public class DeleteOrderCommandHandler : ICommandHandler<DeleteOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;

        public DeleteOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task<IResponse<Guid>> Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Order order = await _orderRepository.GetByIdAsync(command.Id);
            if (order == null) throw new Exception("Order not found!!");

            _orderRepository.Remove(order);
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(order.Id);
        }
    }
}
