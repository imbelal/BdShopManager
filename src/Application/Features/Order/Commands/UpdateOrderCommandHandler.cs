using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Order.Commands
{
    public class UpdateOrderCommandHandler : ICommandHandler<UpdateOrderCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public UpdateOrderCommandHandler(IOrderRepository orderRepository, IReadOnlyApplicationDbContext context)
        {
            _orderRepository = orderRepository;
            _context = context;
        }

        public async Task<IResponse<bool>> Handle(UpdateOrderCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Order? order = await _orderRepository.GetByIdAsync(command.Id, cancellationToken);
            if (order == null)
            {
                throw new Exception("Order not found!");
            }

            Domain.Entities.Customer? customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == command.CustomerId, cancellationToken);
            if (customer == null)
            {
                throw new Exception("Customer not found!");
            }

            order.Update(command.CustomerId, command.TotalPrice, command.TotalPaid, command.Remark, command.OrderDetails, command.TaxPercentage);
            _orderRepository.Update(order);
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(true);
        }
    }
}
