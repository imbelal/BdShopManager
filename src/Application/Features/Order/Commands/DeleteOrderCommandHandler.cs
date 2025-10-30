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
            Domain.Entities.Order? order = await _orderRepository.GetByIdAsync(command.Id, cancellationToken);
            if (order == null) throw new Common.Exceptions.BusinessLogicException("Order not found!!");

            order.Delete();
            _orderRepository.Update(order);
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(order.Id);
        }
    }
}
