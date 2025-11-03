using Common.Pagination;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Customer.Queries
{
    public class GetAllCustomersWithPaginationQueryHandler : IQueryHandler<GetAllCustomersWithPaginationQuery, Pagination<CustomerDto>>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetAllCustomersWithPaginationQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<Pagination<CustomerDto>>> Handle(GetAllCustomersWithPaginationQuery query, CancellationToken cancellationToken)
        {
            var customersQuery = _context.Customers
                .Where(c => !c.IsDeleted);

            // Search functionality
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var searchLower = query.SearchTerm.ToLower();
                customersQuery = customersQuery.Where(c =>
                    c.FirstName.ToLower().Contains(searchLower) ||
                    c.LastName.ToLower().Contains(searchLower) ||
                    c.Email.ToLower().Contains(searchLower) ||
                    c.ContactNo.ToLower().Contains(searchLower) ||
                    c.Address.ToLower().Contains(searchLower)
                );
            }

            // Get customers with their raw data first
            var customersRaw = await customersQuery
                .Select(c => new
                {
                    c.Id,
                    c.FirstName,
                    c.LastName,
                    c.Email,
                    c.ContactNo,
                    c.Address,
                    c.Remark,
                    c.CreatedBy,
                    c.CreatedUtcDate,
                })
                .ToListAsync(cancellationToken);

            // Calculate statistics and build CustomerDto list
            var customersDto = new List<CustomerDto>();

            foreach (var customer in customersRaw)
            {
                var customerSales = _context.Sales
                    .Where(s => s.CustomerId == customer.Id && !s.IsDeleted)
                    .ToList();

                var customerDto = new CustomerDto
                {
                    Id = customer.Id.ToString(),
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    ContactNo = customer.ContactNo,
                    Address = customer.Address,
                    Remark = customer.Remark,
                    CreatedBy = customer.CreatedBy,
                    CreatedDate = customer.CreatedUtcDate.ToString(),
                    TotalSales = customerSales.Count,
                    TotalSalesAmount = customerSales.Sum(s => s.GrandTotal),
                    TotalDueAmount = customerSales.Sum(s => s.GrandTotal - s.TotalPaid),
                    LastSaleDate = customerSales.Any()
                        ? (DateTime?)customerSales.OrderByDescending(s => s.CreatedUtcDate).First().CreatedUtcDate.DateTime
                        : null,
                    FormattedTotalSalesAmount = "", // Will be formatted in the application layer if needed
                    FormattedTotalDueAmount = "",    // Will be formatted in the application layer if needed
                    FormattedLastSaleDate = ""       // Will be formatted in the application layer if needed
                };

                customersDto.Add(customerDto);
            }

            // Apply ordering after materialization
            customersDto = customersDto.OrderByDescending(c => c.CreatedDate).ToList();

            // Create pagination by filtering the full list with pagination logic
            var allCustomersQueryable = customersDto.AsQueryable();
            var pagination = new Pagination<CustomerDto>(allCustomersQueryable, query.PageNumber, query.PageSize);

            return Response.Success(pagination);
        }
    }
}