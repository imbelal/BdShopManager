using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Reports.Queries
{
    public class GetFinancialMetricsQueryHandler : IQueryHandler<GetFinancialMetricsQuery, FinancialMetricsDto>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetFinancialMetricsQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<FinancialMetricsDto>> Handle(GetFinancialMetricsQuery request, CancellationToken cancellationToken)
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

            // Get sales data
            var salesQuery = _context.Sales
                .Where(s => s.CreatedUtcDate >= request.StartDate && s.CreatedUtcDate <= request.EndDate);

            var totalRevenue = await salesQuery.SumAsync(s => s.TotalPrice, cancellationToken);
            var totalSalesTransactions = await salesQuery.CountAsync(cancellationToken);

            // Get sales items for cost calculation
            var salesItemsQuery = _context.SalesItems
                .Where(si => salesQuery.Any(s => s.Id == si.SalesId));

            var totalCostOfGoodsSold = await salesItemsQuery.SumAsync(si => si.TotalCost, cancellationToken);

            // Get expenses data
            var totalExpenses = await _context.Expenses
                .Where(e => e.ExpenseDate >= request.StartDate && e.ExpenseDate <= request.EndDate)
                .SumAsync(e => e.Amount, cancellationToken);

            // Calculate basic profitability metrics
            var grossProfit = totalRevenue - totalCostOfGoodsSold;
            var netProfit = grossProfit - totalExpenses;

            var grossProfitMargin = totalRevenue > 0 ? (grossProfit / totalRevenue) * 100 : 0;
            var netProfitMargin = totalRevenue > 0 ? (netProfit / totalRevenue) * 100 : 0;
            var operatingMargin = totalRevenue > 0 ? ((grossProfit - totalExpenses) / totalRevenue) * 100 : 0;

            // Inventory metrics
            var totalInventoryValue = await _context.StockTransactions
                .Where(st => st.Type == Domain.Enums.StockTransactionType.IN)
                .SumAsync(st => st.Quantity * st.UnitCost, cancellationToken);

            var currentInventory = await _context.Products
                .Where(p => !p.IsDeleted && p.Status == Domain.Enums.ProductStatus.Active)
                .SumAsync(p => p.StockQuantity * p.CostPrice, cancellationToken);

            // Calculate inventory turnover (COGS / Average Inventory)
            var averageInventory = (totalInventoryValue + currentInventory) / 2;
            var inventoryTurnover = averageInventory > 0 ? totalCostOfGoodsSold / averageInventory : 0;

            // Working capital calculation (Current Assets - Current Liabilities)
            // Simplified calculation - would need more comprehensive data in real scenario
            var currentAssets = currentInventory + (await salesQuery.SumAsync(s => s.TotalPaid, cancellationToken));
            var currentLiabilities = await _context.Purchases
                .Where(p => p.PurchaseDate >= request.StartDate && p.PurchaseDate <= request.EndDate)
                .SumAsync(p => p.TotalCost, cancellationToken);

            var workingCapital = currentAssets - currentLiabilities;

            var currentRatio = currentLiabilities > 0 ? currentAssets / currentLiabilities : 0;
            var quickRatio = currentLiabilities > 0 ? (currentAssets - currentInventory) / currentLiabilities : 0;

            // Cash flow (simplified - would need more detailed cash flow data)
            var cashFlowFromOperations = netProfit; // Simplified calculation

            // Additional ratios
            var totalAssets = currentAssets; // Simplified
            var totalEquity = totalAssets - currentLiabilities; // Simplified

            var returnOnAssets = totalAssets > 0 ? (netProfit / totalAssets) * 100 : 0;
            var returnOnEquity = totalEquity > 0 ? (netProfit / totalEquity) * 100 : 0;
            var debtToEquityRatio = totalEquity > 0 ? (currentLiabilities / totalEquity) : 0;

            // Accounts receivable turnover (simplified)
            var accountsReceivable = await salesQuery
                .SumAsync(s => s.TotalPrice - s.TotalPaid, cancellationToken);
            var accountsReceivableTurnover = accountsReceivable > 0 ? totalRevenue / accountsReceivable : 0;

            // EPS and P/E (placeholder values - would need more data)
            var earningsPerShare = totalEquity > 0 ? (netProfit / totalEquity) * 100 : 0;
            var priceToEarningsRatio = earningsPerShare > 0 ? 1 / earningsPerShare : 0;

            // Key Performance Indicators
            var keyPerformanceIndicators = new List<FinancialKpiDto>();
            if (request.IncludeKPIs)
            {
                keyPerformanceIndicators = new List<FinancialKpiDto>
                {
                    new FinancialKpiDto
                    {
                        Name = "Revenue Growth",
                        Value = 0, // Would need previous period data
                        Target = 10,
                        Variance = -10,
                        VariancePercentage = -100,
                        Status = "Critical",
                        Unit = "%",
                        FormattedValue = "0%",
                        FormattedTarget = "10%",
                        FormattedVariance = "-10%"
                    },
                    new FinancialKpiDto
                    {
                        Name = "Profit Margin",
                        Value = netProfitMargin,
                        Target = 15,
                        Variance = netProfitMargin - 15,
                        VariancePercentage = netProfitMargin > 0 ? ((netProfitMargin - 15) / 15) * 100 : -100,
                        Status = netProfitMargin >= 15 ? "Good" : netProfitMargin >= 10 ? "Warning" : "Critical",
                        Unit = "%",
                        FormattedValue = $"{Math.Round(netProfitMargin, 2)}%",
                        FormattedTarget = "15%",
                        FormattedVariance = $"{Math.Round(netProfitMargin - 15, 2)}%"
                    },
                    new FinancialKpiDto
                    {
                        Name = "Inventory Turnover",
                        Value = inventoryTurnover,
                        Target = 6,
                        Variance = inventoryTurnover - 6,
                        VariancePercentage = inventoryTurnover > 0 ? ((inventoryTurnover - 6) / 6) * 100 : -100,
                        Status = inventoryTurnover >= 6 ? "Good" : inventoryTurnover >= 4 ? "Warning" : "Critical",
                        Unit = "times",
                        FormattedValue = Math.Round(inventoryTurnover, 2).ToString(),
                        FormattedTarget = "6",
                        FormattedVariance = Math.Round(inventoryTurnover - 6, 2).ToString()
                    }
                };
            }

            // Financial Ratios
            var financialRatios = new List<FinancialRatioDto>();
            if (request.IncludeRatios)
            {
                financialRatios = new List<FinancialRatioDto>
                {
                    new FinancialRatioDto
                    {
                        Name = "Current Ratio",
                        Category = "Liquidity",
                        Value = currentRatio,
                        IndustryAverage = 1.5m,
                        Variance = currentRatio - 1.5m,
                        Interpretation = currentRatio >= 1.5m ? "Good liquidity" : "Poor liquidity",
                        Unit = "ratio",
                        FormattedValue = Math.Round(currentRatio, 2).ToString(),
                        FormattedIndustryAverage = "1.5"
                    },
                    new FinancialRatioDto
                    {
                        Name = "Quick Ratio",
                        Category = "Liquidity",
                        Value = quickRatio,
                        IndustryAverage = 1.0m,
                        Variance = quickRatio - 1.0m,
                        Interpretation = quickRatio >= 1.0m ? "Good immediate liquidity" : "Poor immediate liquidity",
                        Unit = "ratio",
                        FormattedValue = Math.Round(quickRatio, 2).ToString(),
                        FormattedIndustryAverage = "1.0"
                    },
                    new FinancialRatioDto
                    {
                        Name = "Debt to Equity",
                        Category = "Solvency",
                        Value = debtToEquityRatio,
                        IndustryAverage = 0.5m,
                        Variance = debtToEquityRatio - 0.5m,
                        Interpretation = debtToEquityRatio <= 0.5m ? "Low debt risk" : "High debt risk",
                        Unit = "ratio",
                        FormattedValue = Math.Round(debtToEquityRatio, 2).ToString(),
                        FormattedIndustryAverage = "0.5"
                    }
                };
            }

            var result = new FinancialMetricsDto
            {
                PeriodStart = request.StartDate,
                PeriodEnd = request.EndDate,
                TotalRevenue = totalRevenue,
                TotalExpenses = totalExpenses,
                NetProfit = netProfit,
                GrossProfitMargin = Math.Round(grossProfitMargin, 2),
                NetProfitMargin = Math.Round(netProfitMargin, 2),
                OperatingMargin = Math.Round(operatingMargin, 2),
                CurrentRatio = Math.Round(currentRatio, 2),
                QuickRatio = Math.Round(quickRatio, 2),
                InventoryTurnover = Math.Round(inventoryTurnover, 2),
                AccountsReceivableTurnover = Math.Round(accountsReceivableTurnover, 2),
                ReturnOnAssets = Math.Round(returnOnAssets, 2),
                ReturnOnEquity = Math.Round(returnOnEquity, 2),
                DebtToEquityRatio = Math.Round(debtToEquityRatio, 2),
                EarningsPerShare = Math.Round(earningsPerShare, 2),
                PriceToEarningsRatio = Math.Round(priceToEarningsRatio, 2),
                WorkingCapital = Math.Round(workingCapital, 2),
                CashFlowFromOperations = Math.Round(cashFlowFromOperations, 2),
                KeyPerformanceIndicators = keyPerformanceIndicators,
                FinancialRatios = financialRatios,
                FormattedTotalRevenue = totalRevenue.ToString("C"),
                FormattedTotalExpenses = totalExpenses.ToString("C"),
                FormattedNetProfit = netProfit.ToString("C"),
                FormattedWorkingCapital = workingCapital.ToString("C"),
                FormattedCashFlowFromOperations = cashFlowFromOperations.ToString("C")
            };

            return Response.Success(result);
        }
    }
}