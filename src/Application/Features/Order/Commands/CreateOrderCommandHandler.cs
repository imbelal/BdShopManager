using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Order.Commands
{
    public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public CreateOrderCommandHandler(IOrderRepository orderRepository, IReadOnlyApplicationDbContext context)
        {
            _orderRepository = orderRepository;
            _context = context;
        }

        public async Task<IResponse<Guid>> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Customer customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == command.CustomerId, cancellationToken) ??
                throw new Exception("Customer not found!");

            Domain.Entities.Order order = Domain.Entities.Order.CreateOrderWithDetails(command.CustomerId,
                command.TotalPrice,
                command.TotalPaid,
                command.Remark,
                command.OrderDetails);
            _orderRepository.Add(order);
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return Response.Success(order.Id);
        }
    }
}
