using System;
using System.Collections.Generic;

namespace Domain.Dtos
{
    public class ProfitLossDto
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal CostOfGoodsSold { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal GrossProfitMargin { get; set; }
        public List<ExpenseCategoryDto> OperatingExpenses { get; set; } = new();
        public decimal TotalOperatingExpenses { get; set; }
        public decimal OperatingIncome { get; set; }
        public decimal OperatingMargin { get; set; }
        public decimal OtherIncome { get; set; }
        public decimal OtherExpenses { get; set; }
        public decimal NetIncome { get; set; }
        public decimal NetProfitMargin { get; set; }
        public int TotalSalesTransactions { get; set; }
        public int TotalPurchaseTransactions { get; set; }
        public decimal AverageOrderValue { get; set; }
        public string FormattedTotalRevenue { get; set; }
        public string FormattedGrossProfit { get; set; }
        public string FormattedOperatingIncome { get; set; }
        public string FormattedNetIncome { get; set; }
        public string FormattedAverageOrderValue { get; set; }
    }

    public class ExpenseCategoryDto
    {
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public int TransactionCount { get; set; }
        public decimal PercentageOfTotal { get; set; }
        public decimal BudgetAmount { get; set; }
        public decimal Variance { get; set; }
        public decimal VariancePercentage { get; set; }
        public string FormattedAmount { get; set; }
        public string FormattedBudgetAmount { get; set; }
        public string FormattedVariance { get; set; }
    }

    public class FinancialRevenueBreakdownDto
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<RevenueCategoryDto> CategoryBreakdown { get; set; } = new();
        public List<RevenueTrendDto> MonthlyTrends { get; set; } = new();
        public List<TopSellingProductDto> TopProducts { get; set; } = new();
        public List<CustomerRevenueDto> CustomerBreakdown { get; set; } = new();
        public decimal PreviousPeriodRevenue { get; set; }
        public decimal GrowthRate { get; set; }
        public decimal AverageRevenuePerTransaction { get; set; }
        public int TotalTransactions { get; set; }
        public string FormattedTotalRevenue { get; set; }
        public string FormattedGrowthRate { get; set; }
        public string FormattedAverageRevenuePerTransaction { get; set; }
    }

    public class RevenueCategoryDto
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal Revenue { get; set; }
        public int TransactionCount { get; set; }
        public decimal PercentageOfTotal { get; set; }
        public decimal AverageTransactionValue { get; set; }
        public decimal GrowthRate { get; set; }
        public string FormattedRevenue { get; set; }
        public string FormattedAverageTransactionValue { get; set; }
        public string FormattedGrowthRate { get; set; }
    }

    public class RevenueTrendDto
    {
        public DateTime Period { get; set; }
        public decimal Revenue { get; set; }
        public int TransactionCount { get; set; }
        public decimal AverageTransactionValue { get; set; }
        public decimal GrowthRate { get; set; }
        public string FormattedPeriod { get; set; }
        public string FormattedRevenue { get; set; }
        public string FormattedAverageTransactionValue { get; set; }
        public string FormattedGrowthRate { get; set; }
    }

    public class TopSellingProductDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string CategoryName { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
        public decimal Profit { get; set; }
        public decimal ProfitMargin { get; set; }
        public int TransactionCount { get; set; }
        public decimal AverageUnitPrice { get; set; }
        public string FormattedRevenue { get; set; }
        public string FormattedProfit { get; set; }
        public string FormattedAverageUnitPrice { get; set; }
    }

    public class CustomerRevenueDto
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TransactionCount { get; set; }
        public decimal AverageTransactionValue { get; set; }
        public DateTime FirstTransactionDate { get; set; }
        public DateTime LastTransactionDate { get; set; }
        public string CustomerType { get; set; }
        public string FormattedTotalRevenue { get; set; }
        public string FormattedAverageTransactionValue { get; set; }
        public string FormattedFirstTransactionDate { get; set; }
        public string FormattedLastTransactionDate { get; set; }
    }

    public class ExpenseAnalysisDto
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal TotalExpenses { get; set; }
        public List<ExpenseCategoryBreakdownDto> CategoryBreakdown { get; set; } = new();
        public List<ExpenseTrendDto> MonthlyTrends { get; set; } = new();
        public List<ExpenseByVendorDto> VendorBreakdown { get; set; } = new();
        public decimal PreviousPeriodExpenses { get; set; }
        public decimal GrowthRate { get; set; }
        public decimal AverageExpensePerTransaction { get; set; }
        public int TotalTransactions { get; set; }
        public decimal BudgetVsActual { get; set; }
        public decimal BudgetUtilizationPercentage { get; set; }
        public string FormattedTotalExpenses { get; set; }
        public string FormattedGrowthRate { get; set; }
        public string FormattedAverageExpensePerTransaction { get; set; }
        public string FormattedBudgetVsActual { get; set; }
    }

    public class ExpenseCategoryBreakdownDto
    {
        public string CategoryType { get; set; }
        public decimal TotalAmount { get; set; }
        public int TransactionCount { get; set; }
        public decimal PercentageOfTotal { get; set; }
        public decimal AverageTransactionAmount { get; set; }
        public decimal BudgetAmount { get; set; }
        public decimal Variance { get; set; }
        public decimal VariancePercentage { get; set; }
        public string FormattedTotalAmount { get; set; }
        public string FormattedAverageTransactionAmount { get; set; }
        public string FormattedBudgetAmount { get; set; }
        public string FormattedVariance { get; set; }
    }

    public class ExpenseTrendDto
    {
        public DateTime Period { get; set; }
        public decimal TotalAmount { get; set; }
        public int TransactionCount { get; set; }
        public decimal AverageTransactionAmount { get; set; }
        public decimal GrowthRate { get; set; }
        public string FormattedPeriod { get; set; }
        public string FormattedTotalAmount { get; set; }
        public string FormattedAverageTransactionAmount { get; set; }
        public string FormattedGrowthRate { get; set; }
    }

    public class ExpenseByVendorDto
    {
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public decimal TotalAmount { get; set; }
        public int TransactionCount { get; set; }
        public decimal AverageTransactionAmount { get; set; }
        public string CategoryType { get; set; }
        public decimal PercentageOfTotal { get; set; }
        public string FormattedTotalAmount { get; set; }
        public string FormattedAverageTransactionAmount { get; set; }
    }

    public class FinancialMetricsDto
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit { get; set; }
        public decimal GrossProfitMargin { get; set; }
        public decimal NetProfitMargin { get; set; }
        public decimal OperatingMargin { get; set; }
        public decimal CurrentRatio { get; set; }
        public decimal QuickRatio { get; set; }
        public decimal InventoryTurnover { get; set; }
        public decimal AccountsReceivableTurnover { get; set; }
        public decimal ReturnOnAssets { get; set; }
        public decimal ReturnOnEquity { get; set; }
        public decimal DebtToEquityRatio { get; set; }
        public decimal EarningsPerShare { get; set; }
        public decimal PriceToEarningsRatio { get; set; }
        public decimal WorkingCapital { get; set; }
        public decimal CashFlowFromOperations { get; set; }
        public List<FinancialKpiDto> KeyPerformanceIndicators { get; set; } = new();
        public List<FinancialRatioDto> FinancialRatios { get; set; } = new();
        public string FormattedTotalRevenue { get; set; }
        public string FormattedTotalExpenses { get; set; }
        public string FormattedNetProfit { get; set; }
        public string FormattedWorkingCapital { get; set; }
        public string FormattedCashFlowFromOperations { get; set; }
    }

    public class FinancialKpiDto
    {
        public string Name { get; set; }
        public decimal Value { get; set; }
        public decimal Target { get; set; }
        public decimal Variance { get; set; }
        public decimal VariancePercentage { get; set; }
        public string Status { get; set; } // "Good", "Warning", "Critical"
        public string Unit { get; set; }
        public string FormattedValue { get; set; }
        public string FormattedTarget { get; set; }
        public string FormattedVariance { get; set; }
    }

    public class FinancialRatioDto
    {
        public string Name { get; set; }
        public string Category { get; set; } // "Liquidity", "Profitability", "Efficiency", "Solvency"
        public decimal Value { get; set; }
        public decimal IndustryAverage { get; set; }
        public decimal Variance { get; set; }
        public string Interpretation { get; set; }
        public string Unit { get; set; }
        public string FormattedValue { get; set; }
        public string FormattedIndustryAverage { get; set; }
    }
}