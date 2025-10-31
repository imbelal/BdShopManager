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
                throw new Common.Exceptions.BusinessLogicException("Customer not found!");

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

            Domain.Entities.Order order = Domain.Entities.Order.CreateOrderWithDetails(command.CustomerId,
                command.TotalPrice,
                command.TotalPaid,
                command.Remark,
                command.OrderDetails,
                command.TaxPercentage);

            // Generate and set unique order number
            var orderNumber = await GenerateOrderNumber(cancellationToken);
            order.SetOrderNumber(orderNumber);

            _orderRepository.Add(order);
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return Response.Success(order.Id);
        }

        private async Task<string> GenerateOrderNumber(CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            // Get the count of orders created today to determine the sequence number
            var ordersToday = await _context.Orders
                .Where(o => o.CreatedUtcDate >= today && o.CreatedUtcDate < tomorrow)
                .CountAsync(cancellationToken);

            var sequenceNumber = ordersToday + 1;

            // Use domain entity method for order number formatting
            return Domain.Entities.Order.GenerateOrderNumber(today, sequenceNumber);
        }
    }
}
