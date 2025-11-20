using Domain.Enums;

namespace Domain.Dtos
{
    public class DashboardMetricsDto
    {
        // Financial KPIs
        public decimal TotalRevenue { get; set; }
        public decimal CurrentMonthRevenue { get; set; }
        public decimal PreviousMonthRevenue { get; set; }
        public decimal RevenueGrowth { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal CurrentMonthExpenses { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal CurrentMonthProfit { get; set; }
        public decimal AverageOrderValue { get; set; }

        // Sales KPIs
        public int TotalSales { get; set; }
        public int CurrentMonthSales { get; set; }
        public int PreviousMonthSales { get; set; }
        public int SalesGrowth { get; set; }
        public decimal TotalSalesAmount { get; set; }

        // Purchase KPIs
        public int TotalPurchases { get; set; }
        public int CurrentMonthPurchases { get; set; }
        public decimal TotalPurchasesAmount { get; set; }

        // Product & Inventory KPIs
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int LowStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public decimal TotalInventoryValue { get; set; }

        // Customer KPIs
        public int TotalCustomers { get; set; }
        public int NewCustomersThisMonth { get; set; }
        public int ActiveCustomers { get; set; }

        // Order Management
        public int PendingOrders { get; set; }
        public int OrdersToday { get; set; }

        // Profit Metrics
        public decimal GrossProfit { get; set; }
        public decimal NetProfit { get; set; }
        public decimal ProfitMargin { get; set; }

        // Period Information
        public DateTime CurrentPeriodStart { get; set; }
        public DateTime CurrentPeriodEnd { get; set; }
        public DateTime PreviousPeriodStart { get; set; }
        public DateTime PreviousPeriodEnd { get; set; }
    }

    public class RecentSaleDto
    {
        public string Id { get; set; }
        public DateTime SaleDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public SalesStatus Status { get; set; }
        public string CreatedBy { get; set; }
        public string FormattedSaleDate { get; set; }
        public string FormattedTotalAmount { get; set; }
    }

    public class RecentPurchaseDto
    {
        public string Id { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string SupplierName { get; set; }
        public string InvoiceNumber { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public string FormattedPurchaseDate { get; set; }
        public string FormattedTotalAmount { get; set; }
    }

    public class TopProductDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public string ProductCode { get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageUnitPrice { get; set; }
        public int SalesCount { get; set; }
        public decimal GrowthPercentage { get; set; }
        public int CurrentStock { get; set; }
        public string ImageUrl { get; set; }
        public string FormattedTotalRevenue { get; set; }
        public string FormattedAverageUnitPrice { get; set; }
    }

    public class LowStockProductDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public string ProductCode { get; set; }
        public int CurrentStock { get; set; }
        public int MinimumStock { get; set; }
        public int ReorderLevel { get; set; }
        public decimal UnitCost { get; set; }
        public decimal UnitPrice { get; set; }
        public int LastMonthQuantitySold { get; set; }
        public bool IsOutOfStock { get; set; }
        public string Urgency { get; set; }
        public string FormattedUnitCost { get; set; }
        public string FormattedUnitPrice { get; set; }
    }

    public class SalesTrendDto
    {
        public List<DailySalesDto> DailySales { get; set; }
        public List<MonthlySalesDto> MonthlySales { get; set; }
        public decimal GrowthRate { get; set; }
        public int TotalTransactions { get; set; }
        public decimal AverageTransactionValue { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }

    public class DailySalesDto
    {
        public DateTime Date { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalCustomers { get; set; }
        public decimal AverageOrderValue { get; set; }
        public string FormattedDate { get; set; }
        public string FormattedRevenue { get; set; }
        public string FormattedAverageOrderValue { get; set; }
    }

    public class MonthlySalesDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalCustomers { get; set; }
        public decimal AverageOrderValue { get; set; }
        public decimal TotalProfit { get; set; }
        public string FormattedMonth { get; set; }
        public string FormattedRevenue { get; set; }
        public string FormattedAverageOrderValue { get; set; }
        public string FormattedProfit { get; set; }
    }

    public class CustomerAnalyticsDto
    {
        public int TotalCustomers { get; set; }
        public int NewCustomersThisMonth { get; set; }
        public int ActiveCustomersThisMonth { get; set; }
        public int RepeatCustomersThisMonth { get; set; }
        public decimal CustomerRetentionRate { get; set; }
        public decimal AverageCustomerLifetimeValue { get; set; }
        public List<TopCustomerDto> TopCustomers { get; set; }
    }

    public class TopCustomerDto
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime LastOrderDate { get; set; }
        public DateTime FirstOrderDate { get; set; }
        public int DaysSinceLastOrder { get; set; }
        public string CustomerType { get; set; }
        public string FormattedTotalSpent { get; set; }
        public string FormattedAverageOrderValue { get; set; }
        public string FormattedLastOrderDate { get; set; }
    }

    public class InventorySummaryDto
    {
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int InactiveProducts { get; set; }
        public int TotalStockQuantity { get; set; }
        public decimal TotalStockValue { get; set; }
        public int LowStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public decimal AverageStockValue { get; set; }
        public List<CategoryStockDto> CategoryBreakdown { get; set; }
        public decimal StockTurnoverRate { get; set; }
    }

    public class CategoryStockDto
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }
        public int TotalStockQuantity { get; set; }
        public decimal TotalStockValue { get; set; }
        public int LowStockProducts { get; set; }
        public decimal PercentageOfTotalStock { get; set; }
        public string FormattedTotalStockValue { get; set; }
    }

    public class RevenueSummaryDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal CurrentMonthRevenue { get; set; }
        public decimal PreviousMonthRevenue { get; set; }
        public decimal LastYearRevenue { get; set; }
        public decimal YearToDateRevenue { get; set; }
        public decimal RevenueGrowth { get; set; }
        public decimal YearOverYearGrowth { get; set; }
        public decimal MonthOverMonthGrowth { get; set; }
        public List<RevenueBreakdownDto> RevenueBreakdown { get; set; }
    }

    public class RevenueBreakdownDto
    {
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
        public int TransactionCount { get; set; }
        public decimal AverageTransactionValue { get; set; }
        public string FormattedAmount { get; set; }
        public string FormattedAverageTransactionValue { get; set; }
    }
}