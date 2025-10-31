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
                throw new Common.Exceptions.BusinessLogicException("Order not found!");
            }

            Domain.Entities.Customer? customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == command.CustomerId, cancellationToken);
            if (customer == null)
            {
                throw new Common.Exceptions.BusinessLogicException("Customer not found!");
            }

            // Fetch all products to get their cost prices
            var productIds = command.OrderDetails.Select(od => od.ProductId).Distinct().ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p, cancellationToken);

            // Validate all products exist and populate unit costs
            foreach (var orderDetail in command.OrderDetails)
            {
                if (!products.ContainsKey(orderDetail.ProductId))
                {
                    throw new Common.Exceptions.BusinessLogicException($"Product with ID {orderDetail.ProductId} not found!");
                }

                // Populate the unit cost from product's current cost price
                orderDetail.UnitCost = products[orderDetail.ProductId].CostPrice;
            }

            order.Update(command.CustomerId, command.TotalPrice, command.TotalPaid, command.Remark, command.OrderDetails, command.TaxPercentage);
            _orderRepository.Update(order);
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(true);
        }
    }
}
