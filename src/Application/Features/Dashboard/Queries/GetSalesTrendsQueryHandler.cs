using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Common.ResponseWrapper;
using Common.RequestWrapper;
using Domain.Dtos;
using Domain.Interfaces;

namespace Application.Features.Dashboard.Queries
{
    public class GetSalesTrendsQueryHandler : IQueryHandler<GetSalesTrendsQuery, SalesTrendDto>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetSalesTrendsQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<SalesTrendDto>> Handle(GetSalesTrendsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var endDate = DateTime.UtcNow.Date;
                var startDate = endDate.AddDays(-request.Days);

                // Get daily sales data for the specified period
                var dailySalesQuery = _context.Sales
                    .Where(s => !s.IsDeleted && s.CreatedUtcDate.Date >= startDate && s.CreatedUtcDate.Date <= endDate)
                    .GroupBy(s => s.CreatedUtcDate.Date)
                    .Select(g => new DailySalesDto
                    {
                        Date = g.Key,
                        TotalRevenue = g.Sum(s => s.TotalPrice),
                        TotalSales = g.Count(),
                        AverageOrderValue = g.Average(s => s.TotalPrice),
                        TotalCustomers = g.Select(s => s.CustomerId).Distinct().Count()
                    })
                    .OrderBy(g => g.Date);

                var dailySales = await dailySalesQuery.ToListAsync(cancellationToken);

                // Get monthly comparison data
                var currentMonthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                var previousMonthStart = currentMonthStart.AddMonths(-1);
                var currentMonthEnd = DateTime.UtcNow;

                // Current month sales
                var currentMonthSales = await _context.Sales
                    .Where(s => !s.IsDeleted && s.CreatedUtcDate >= currentMonthStart && s.CreatedUtcDate <= currentMonthEnd)
                    .ToListAsync(cancellationToken);

                // Previous month sales
                var previousMonthSales = await _context.Sales
                    .Where(s => !s.IsDeleted && s.CreatedUtcDate >= previousMonthStart && s.CreatedUtcDate < currentMonthStart)
                    .ToListAsync(cancellationToken);

                // Calculate monthly breakdown
                var monthlyBreakdown = new List<MonthlySalesDto>
                {
                    new MonthlySalesDto
                    {
                        MonthName = previousMonthStart.ToString("MMM yyyy"),
                        TotalRevenue = previousMonthSales.Sum(s => s.TotalPrice),
                        TotalSales = previousMonthSales.Count,
                        AverageOrderValue = previousMonthSales.Any() ? previousMonthSales.Average(s => s.TotalPrice) : 0,
                        Year = previousMonthStart.Year,
                        Month = previousMonthStart.Month
                    },
                    new MonthlySalesDto
                    {
                        MonthName = currentMonthStart.ToString("MMM yyyy"),
                        TotalRevenue = currentMonthSales.Sum(s => s.TotalPrice),
                        TotalSales = currentMonthSales.Count,
                        AverageOrderValue = currentMonthSales.Any() ? currentMonthSales.Average(s => s.TotalPrice) : 0,
                        Year = currentMonthStart.Year,
                        Month = currentMonthStart.Month
                    }
                };

                // Create sales trend result
                var salesTrend = new SalesTrendDto
                {
                    DailySales = dailySales,
                    MonthlySales = monthlyBreakdown,
                    GrowthRate = previousMonthSales.Any() ?
                        ((currentMonthSales.Sum(s => s.TotalPrice) - previousMonthSales.Sum(s => s.TotalPrice))
                        / previousMonthSales.Sum(s => s.TotalPrice) * 100) : 0,
                    TotalTransactions = dailySales.Sum(d => d.TotalSales),
                    AverageTransactionValue = dailySales.Any() ? dailySales.Average(d => d.AverageOrderValue) : 0,
                    PeriodStart = startDate,
                    PeriodEnd = endDate
                };

                return Response.Success(salesTrend);
            }
            catch (Exception ex)
            {
                return Response.Fail<SalesTrendDto>($"Error retrieving sales trends: {ex.Message}");
            }
        }
    }
}