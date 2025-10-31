using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Sales.Commands
{
    public class CreateSalesCommandHandler : ICommandHandler<CreateSalesCommand, Guid>
    {
        private readonly ISalesRepository _salesRepository;
        private readonly IReadOnlyApplicationDbContext _context;

        public CreateSalesCommandHandler(ISalesRepository salesRepository, IReadOnlyApplicationDbContext context)
        {
            _salesRepository = salesRepository;
            _context = context;
        }

        public async Task<IResponse<Guid>> Handle(CreateSalesCommand command, CancellationToken cancellationToken)
        {
            Domain.Entities.Customer customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == command.CustomerId, cancellationToken) ??
                throw new Common.Exceptions.BusinessLogicException("Customer not found!");

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

            Domain.Entities.Sales sales = Domain.Entities.Sales.CreateSalesWithItems(command.CustomerId,
                command.TotalPrice,
                command.TotalPaid,
                command.Remark,
                command.SalesItems,
                command.TaxPercentage);

            // Generate and set unique sales number
            var salesNumber = await GenerateSalesNumber(cancellationToken);
            sales.SetSalesNumber(salesNumber);

            _salesRepository.Add(sales);
            await _salesRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return Response.Success(sales.Id);
        }

        private async Task<string> GenerateSalesNumber(CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            // Get the count of sales created today to determine the sequence number
            var salesToday = await _context.Sales
                .Where(o => o.CreatedUtcDate >= today && o.CreatedUtcDate < tomorrow)
                .CountAsync(cancellationToken);

            var sequenceNumber = salesToday + 1;

            // Use domain entity method for sales number formatting
            return Domain.Entities.Sales.GenerateSalesNumber(today, sequenceNumber);
        }
    }
}
