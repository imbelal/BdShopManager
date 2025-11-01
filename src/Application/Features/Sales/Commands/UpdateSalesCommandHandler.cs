using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Sales.Commands
{
    public class UpdateSalesCommandHandler : ICommandHandler<UpdateSalesCommand, bool>
    {
        private readonly ISalesRepository _salesRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public UpdateSalesCommandHandler(ISalesRepository salesRepository, IReadOnlyApplicationDbContext context)
        {
            _salesRepository = salesRepository;
            _context = context;
        }

        public async Task<IResponse<bool>> Handle(UpdateSalesCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Sales? sales = await _salesRepository.GetByIdAsync(command.Id, cancellationToken);
            if (sales == null)
            {
                throw new Common.Exceptions.BusinessLogicException("Sales not found!");
            }

            // Prevent editing sales when it's fully paid
            if (sales.Status == Domain.Enums.SalesStatus.Paid)
            {
                throw new Common.Exceptions.BusinessLogicException("Cannot edit sales that is fully paid!");
            }

            Domain.Entities.Customer? customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == command.CustomerId, cancellationToken);
            if (customer == null)
            {
                throw new Common.Exceptions.BusinessLogicException("Customer not found!");
            }

            // Fetch all products to get their cost prices
            var productIds = command.SalesItems.Select(od => od.ProductId).Distinct().ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p, cancellationToken);

            // Validate all products exist and populate unit costs
            foreach (var salesItem in command.SalesItems)
            {
                if (!products.ContainsKey(salesItem.ProductId))
                {
                    throw new Common.Exceptions.BusinessLogicException($"Product with ID {salesItem.ProductId} not found!");
                }

                // Populate the unit cost from product's current cost price
                salesItem.UnitCost = products[salesItem.ProductId].CostPrice;
            }

            sales.Update(command.CustomerId, command.TotalPrice, command.DiscountPercentage, command.TotalPaid, command.Remark, command.SalesItems, command.TaxPercentage);
            _salesRepository.Update(sales);
            await _salesRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Success(true);
        }
    }
}
