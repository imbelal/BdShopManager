using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Reports.Queries
{
    public class GetRevenueBreakdownQueryHandler : IQueryHandler<GetRevenueBreakdownQuery, FinancialRevenueBreakdownDto>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetRevenueBreakdownQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<FinancialRevenueBreakdownDto>> Handle(GetRevenueBreakdownQuery request, CancellationToken cancellationToken)
        {
            // Validate date range
            if (request.StartDate > request.EndDate)
            {
                throw new Common.Exceptions.BusinessLogicException("Start date cannot be greater than end date");
            }

            // Limit date range to 1 year
            if ((request.EndDate - request.StartDate).TotalDays > 365)
            {
                throw new Common.Exceptions.BusinessLogicException("Date range cannot exceed 1 year");
            }

            var baseQuery = _context.Sales
                .Where(s => s.CreatedUtcDate >= request.StartDate && s.CreatedUtcDate <= request.EndDate);

            // Get total revenue
            var totalRevenue = await baseQuery.SumAsync(s => s.TotalPrice, cancellationToken);
            var totalTransactions = await baseQuery.CountAsync(cancellationToken);

            // Category breakdown
            var categoryBreakdown = await (from s in baseQuery
                                         join si in _context.SalesItems on s.Id equals si.SalesId
                                         join p in _context.Products on si.ProductId equals p.Id
                                         join c in _context.Categories on p.CategoryId equals c.Id
                                         group new { si.TotalPrice, si.Quantity, s.CreatedUtcDate } by new { c.Id, c.Title } into g
                                         select new RevenueCategoryDto
                                         {
                                             CategoryId = g.Key.Id.ToString(),
                                             CategoryName = g.Key.Title,
                                             Revenue = g.Sum(x => x.TotalPrice),
                                             TransactionCount = g.Count(),
                                             AverageTransactionValue = g.Average(x => x.TotalPrice),
                                             GrowthRate = 0, // Would need previous period data for calculation
                                             FormattedRevenue = g.Sum(x => x.TotalPrice).ToString("C"),
                                             FormattedAverageTransactionValue = g.Average(x => x.TotalPrice).ToString("C"),
                                             FormattedGrowthRate = "0%"
                                         }).ToListAsync(cancellationToken);

            // Calculate percentages
            foreach (var category in categoryBreakdown)
            {
                category.PercentageOfTotal = totalRevenue > 0 ? (category.Revenue / totalRevenue) * 100 : 0;
            }

            // Monthly trends
            var monthlyTrends = new List<RevenueTrendDto>();
            var currentDate = new DateTime(request.StartDate.Year, request.StartDate.Month, 1);

            while (currentDate <= request.EndDate)
            {
                var periodEnd = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
                periodEnd = periodEnd > request.EndDate ? request.EndDate : periodEnd;

                var periodRevenue = await baseQuery
                    .Where(s => s.CreatedUtcDate >= currentDate && s.CreatedUtcDate <= periodEnd)
                    .SumAsync(s => s.TotalPrice, cancellationToken);

                var periodTransactions = await baseQuery
                    .Where(s => s.CreatedUtcDate >= currentDate && s.CreatedUtcDate <= periodEnd)
                    .CountAsync(cancellationToken);

                var avgTransactionValue = periodTransactions > 0 ? periodRevenue / periodTransactions : 0;

                monthlyTrends.Add(new RevenueTrendDto
                {
                    Period = currentDate,
                    Revenue = periodRevenue,
                    TransactionCount = periodTransactions,
                    AverageTransactionValue = avgTransactionValue,
                    GrowthRate = 0, // Would need previous period data
                    FormattedPeriod = currentDate.ToString("MMM yyyy"),
                    FormattedRevenue = periodRevenue.ToString("C"),
                    FormattedAverageTransactionValue = avgTransactionValue.ToString("C"),
                    FormattedGrowthRate = "0%"
                });

                currentDate = currentDate.AddMonths(1);
            }

            // Top selling products
            var topProducts = new List<TopSellingProductDto>();
            if (request.IncludeTopProducts)
            {
                topProducts = await (from s in baseQuery
                                    join si in _context.SalesItems on s.Id equals si.SalesId
                                    join p in _context.Products on si.ProductId equals p.Id
                                    join c in _context.Categories on p.CategoryId equals c.Id
                                    group new { si, p, c } by new { p.Id, ProductTitle = p.Title, CategoryTitle = c.Title } into g
                                    select new TopSellingProductDto
                                    {
                                        ProductId = g.Key.Id.ToString(),
                                        ProductName = g.Key.ProductTitle,
                                        ProductCode = "", // Product doesn't have Code property
                                        CategoryName = g.FirstOrDefault().c.Title,
                                        QuantitySold = g.Sum(x => x.si.Quantity),
                                        Revenue = g.Sum(x => x.si.TotalPrice),
                                        Profit = g.Sum(x => x.si.TotalPrice - x.si.TotalCost),
                                        ProfitMargin = g.Sum(x => x.si.TotalPrice) > 0 ?
                                            ((g.Sum(x => x.si.TotalPrice - x.si.TotalCost) / g.Sum(x => x.si.TotalPrice)) * 100) : 0,
                                        TransactionCount = g.Count(),
                                        AverageUnitPrice = g.Sum(x => x.si.TotalPrice) > 0 ?
                                            g.Sum(x => x.si.TotalPrice) / g.Sum(x => x.si.Quantity) : 0,
                                        FormattedRevenue = g.Sum(x => x.si.TotalPrice).ToString("C"),
                                        FormattedProfit = g.Sum(x => x.si.TotalPrice - x.si.TotalCost).ToString("C"),
                                        FormattedAverageUnitPrice = (g.Sum(x => x.si.TotalPrice) > 0 ?
                                            g.Sum(x => x.si.TotalPrice) / g.Sum(x => x.si.Quantity) : 0).ToString("C")
                                    })
                                    .OrderByDescending(p => p.Revenue)
                                    .Take(request.TopProductsCount)
                                    .ToListAsync(cancellationToken);
            }

            // Customer breakdown
            var customerBreakdown = new List<CustomerRevenueDto>();
            if (request.IncludeCustomerBreakdown)
            {
                // First get the raw data from database
                var customerData = await (from s in baseQuery
                                         join c in _context.Customers on s.CustomerId equals c.Id
                                         group s by new { c.Id, c.FirstName, c.LastName, c.ContactNo } into g
                                         select new
                                         {
                                             CustomerId = g.Key.Id,
                                             CustomerName = $"{g.Key.FirstName} {g.Key.LastName}",
                                             Phone = g.Key.ContactNo,
                                             TotalRevenue = g.Sum(x => x.TotalPrice),
                                             TransactionCount = g.Count(),
                                             AverageTransactionValue = g.Average(x => x.TotalPrice),
                                             FirstTransactionDate = g.Min(x => x.CreatedUtcDate),
                                             LastTransactionDate = g.Max(x => x.CreatedUtcDate)
                                         })
                                         .OrderByDescending(c => c.TotalRevenue)
                                         .Take(request.TopCustomersCount)
                                         .ToListAsync(cancellationToken);

                // Then format the data in memory
                customerBreakdown = customerData.Select(c => new CustomerRevenueDto
                {
                    CustomerId = c.CustomerId.ToString(),
                    CustomerName = c.CustomerName,
                    Phone = c.Phone,
                    TotalRevenue = c.TotalRevenue,
                    TransactionCount = c.TransactionCount,
                    AverageTransactionValue = c.AverageTransactionValue,
                    FirstTransactionDate = c.FirstTransactionDate.DateTime,
                    LastTransactionDate = c.LastTransactionDate.DateTime,
                    CustomerType = c.TransactionCount > 10 ? "Regular" : c.TransactionCount > 1 ? "Repeat" : "New",
                    FormattedTotalRevenue = c.TotalRevenue.ToString("C"),
                    FormattedAverageTransactionValue = c.AverageTransactionValue.ToString("C"),
                    FormattedFirstTransactionDate = c.FirstTransactionDate.DateTime.ToString("MMM dd, yyyy"),
                    FormattedLastTransactionDate = c.LastTransactionDate.DateTime.ToString("MMM dd, yyyy")
                }).ToList();
            }

            // Previous period revenue for growth calculation
            var previousPeriodStart = request.StartDate.AddMonths(-1);
            var previousPeriodEnd = request.StartDate.AddDays(-1);
            var previousPeriodRevenue = await _context.Sales
                .Where(s => s.CreatedUtcDate >= previousPeriodStart && s.CreatedUtcDate <= previousPeriodEnd)
                .SumAsync(s => s.TotalPrice, cancellationToken);

            var growthRate = previousPeriodRevenue > 0 ?
                ((totalRevenue - previousPeriodRevenue) / previousPeriodRevenue) * 100 : 0;

            var averageRevenuePerTransaction = totalTransactions > 0 ? totalRevenue / totalTransactions : 0;

            var result = new FinancialRevenueBreakdownDto
            {
                PeriodStart = request.StartDate,
                PeriodEnd = request.EndDate,
                TotalRevenue = totalRevenue,
                CategoryBreakdown = categoryBreakdown,
                MonthlyTrends = monthlyTrends,
                TopProducts = topProducts,
                CustomerBreakdown = customerBreakdown,
                PreviousPeriodRevenue = previousPeriodRevenue,
                GrowthRate = Math.Round(growthRate, 2),
                AverageRevenuePerTransaction = Math.Round(averageRevenuePerTransaction, 2),
                TotalTransactions = totalTransactions,
                FormattedTotalRevenue = totalRevenue.ToString("C"),
                FormattedGrowthRate = $"{Math.Round(growthRate, 2)}%",
                FormattedAverageRevenuePerTransaction = averageRevenuePerTransaction.ToString("C")
            };

            return Response.Success(result);
        }
    }
}