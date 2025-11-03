using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Dashboard.Queries
{
    public class GetDashboardMetricsQueryHandler : IQueryHandler<GetDashboardMetricsQuery, DashboardMetricsDto>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetDashboardMetricsQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<DashboardMetricsDto>> Handle(GetDashboardMetricsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var now = DateTime.UtcNow;
                var currentMonthStart = request.StartDate ?? new DateTime(now.Year, now.Month, 1);
                var currentMonthEnd = request.EndDate ?? now;
                var previousMonthStart = currentMonthStart.AddMonths(-1);
                var previousMonthEnd = currentMonthStart.AddDays(-1);

                // Get all sales data
                var salesQuery = _context.Sales.Where(s => !s.IsDeleted);
                var currentMonthSales = salesQuery.Where(s => s.CreatedUtcDate >= currentMonthStart && s.CreatedUtcDate <= currentMonthEnd);
                var previousMonthSales = salesQuery.Where(s => s.CreatedUtcDate >= previousMonthStart && s.CreatedUtcDate <= previousMonthEnd);

                // Get all purchases data
                var purchasesQuery = _context.Purchases.Where(p => !p.IsDeleted);
                var currentMonthPurchases = purchasesQuery.Where(p => p.PurchaseDate >= currentMonthStart && p.PurchaseDate <= currentMonthEnd);

                // Get product data
                var productsQuery = _context.Products.Where(p => !p.IsDeleted);

                // Financial calculations
                var allSalesTotal = await salesQuery.SumAsync(s => s.TotalPrice, cancellationToken);
                var currentMonthRevenue = await currentMonthSales.SumAsync(s => s.TotalPrice, cancellationToken);
                var previousMonthRevenue = await previousMonthSales.SumAsync(s => s.TotalPrice, cancellationToken);

                // Get sales items for profit calculations
                var currentMonthSalesItems = await _context.SalesItems
                    .Join(currentMonthSales, si => si.SalesId, s => s.Id, (si, s) => si)
                    .ToListAsync(cancellationToken);

                var allSalesItems = await _context.SalesItems
                    .Join(salesQuery, si => si.SalesId, s => s.Id, (si, s) => si)
                    .ToListAsync(cancellationToken);

                // Calculate profit
                var currentMonthCost = currentMonthSalesItems.Sum(si => si.Quantity * si.UnitCost);
                var allSalesCost = allSalesItems.Sum(si => si.Quantity * si.UnitCost);

                var totalExpenses = await purchasesQuery.SumAsync(p => p.TotalCost, cancellationToken);
                var currentMonthExpenses = await currentMonthPurchases.SumAsync(p => p.TotalCost, cancellationToken);

                // Count calculations
                var totalSales = await salesQuery.CountAsync(cancellationToken);
                var currentMonthSalesCount = await currentMonthSales.CountAsync(cancellationToken);
                var previousMonthSalesCount = await previousMonthSales.CountAsync(cancellationToken);

                var totalPurchases = await purchasesQuery.CountAsync(cancellationToken);
                var currentMonthPurchasesCount = await currentMonthPurchases.CountAsync(cancellationToken);

                // Product counts
                var totalProducts = await productsQuery.CountAsync(cancellationToken);
                var activeProducts = await productsQuery.CountAsync(p => p.Status == Domain.Enums.ProductStatus.Active, cancellationToken);
                // Note: MinimumStock doesn't exist on Product entity, using a default threshold of 10
                var lowStockProducts = await productsQuery.CountAsync(p => p.StockQuantity <= 10, cancellationToken);
                var outOfStockProducts = await productsQuery.CountAsync(p => p.StockQuantity == 0, cancellationToken);

                // Calculate total inventory value
                var totalInventoryValue = await productsQuery.SumAsync(p => p.StockQuantity * p.CostPrice, cancellationToken);

                // Customer counts
                var totalCustomers = await _context.Customers.CountAsync(c => !c.IsDeleted, cancellationToken);
                var newCustomersThisMonth = await _context.Customers
                    .CountAsync(c => !c.IsDeleted && c.CreatedUtcDate >= currentMonthStart && c.CreatedUtcDate <= currentMonthEnd, cancellationToken);

                var activeCustomers = await currentMonthSales
                    .Select(s => s.CustomerId)
                    .Distinct()
                    .CountAsync(cancellationToken);

                // Pending orders (sales with status other than paid)
                var pendingOrders = await salesQuery
                    .CountAsync(s => s.Status != Domain.Enums.SalesStatus.Paid, cancellationToken);

                // Orders today
                var todayStart = DateTime.UtcNow.Date;
                var ordersToday = await salesQuery
                    .CountAsync(s => s.CreatedUtcDate >= todayStart, cancellationToken);

                // Calculate growth rates
                var revenueGrowth = previousMonthRevenue > 0 ?
                    ((currentMonthRevenue - previousMonthRevenue) / previousMonthRevenue) * 100 : 0;

                var salesGrowth = previousMonthSalesCount > 0 ?
                    ((currentMonthSalesCount - previousMonthSalesCount) / (decimal)previousMonthSalesCount) * 100 : 0;

                // Calculate profit metrics
                var currentMonthProfit = currentMonthRevenue - currentMonthCost;
                var totalProfit = allSalesTotal - allSalesCost;
                var profitMargin = allSalesTotal > 0 ? (totalProfit / allSalesTotal) * 100 : 0;

                // Average order value
                var averageOrderValue = totalSales > 0 ? allSalesTotal / totalSales : 0;

                var metrics = new DashboardMetricsDto
                {
                    // Financial KPIs
                    TotalRevenue = allSalesTotal,
                    CurrentMonthRevenue = currentMonthRevenue,
                    PreviousMonthRevenue = previousMonthRevenue,
                    RevenueGrowth = Math.Round(revenueGrowth, 2),
                    TotalExpenses = totalExpenses,
                    CurrentMonthExpenses = currentMonthExpenses,
                    TotalProfit = totalProfit,
                    CurrentMonthProfit = currentMonthProfit,
                    AverageOrderValue = Math.Round(averageOrderValue, 2),

                    // Sales KPIs
                    TotalSales = totalSales,
                    CurrentMonthSales = currentMonthSalesCount,
                    PreviousMonthSales = previousMonthSalesCount,
                    SalesGrowth = (int)Math.Round(salesGrowth, 2),
                    TotalSalesAmount = allSalesTotal,

                    // Purchase KPIs
                    TotalPurchases = totalPurchases,
                    CurrentMonthPurchases = currentMonthPurchasesCount,
                    TotalPurchasesAmount = totalExpenses,

                    // Product & Inventory KPIs
                    TotalProducts = totalProducts,
                    ActiveProducts = activeProducts,
                    LowStockProducts = lowStockProducts,
                    OutOfStockProducts = outOfStockProducts,
                    TotalInventoryValue = totalInventoryValue,

                    // Customer KPIs
                    TotalCustomers = totalCustomers,
                    NewCustomersThisMonth = newCustomersThisMonth,
                    ActiveCustomers = activeCustomers,

                    // Order Management
                    PendingOrders = pendingOrders,
                    OrdersToday = ordersToday,

                    // Profit Metrics
                    GrossProfit = totalProfit + totalExpenses, // Revenue before expenses
                    NetProfit = totalProfit, // Revenue after cost of goods sold
                    ProfitMargin = Math.Round(profitMargin, 2),

                    // Period Information
                    CurrentPeriodStart = currentMonthStart,
                    CurrentPeriodEnd = currentMonthEnd,
                    PreviousPeriodStart = previousMonthStart,
                    PreviousPeriodEnd = previousMonthEnd
                };

                return Response.Success(metrics);
            }
            catch (Exception ex)
            {
                return Response.Fail<DashboardMetricsDto>($"Error calculating dashboard metrics: {ex.Message}");
            }
        }
    }
}