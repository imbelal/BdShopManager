using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Reports.Queries
{
    public class GetProfitLossQueryHandler : IQueryHandler<GetProfitLossQuery, ProfitLossDto>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetProfitLossQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<ProfitLossDto>> Handle(GetProfitLossQuery request, CancellationToken cancellationToken)
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

            // Get sales data with cost information
            var salesQuery = _context.Sales
                .Where(s => s.CreatedUtcDate >= request.StartDate && s.CreatedUtcDate <= request.EndDate);

            var salesData = await (from s in salesQuery
                                 join si in _context.SalesItems on s.Id equals si.SalesId into salesItems
                                 from si in salesItems.DefaultIfEmpty()
                                 group new { s, si } by s.Id into g
                                 select new
                                 {
                                     SalesId = g.Key,
                                     TotalRevenue = g.FirstOrDefault().s.TotalPrice,
                                     TotalCost = g.Sum(x => x.si != null ? x.si.TotalCost : 0),
                                     CreatedDate = g.FirstOrDefault().s.CreatedUtcDate
                                 }).ToListAsync(cancellationToken);

            // Get expenses data
            var expensesData = await _context.Expenses
                .Where(e => e.ExpenseDate >= request.StartDate && e.ExpenseDate <= request.EndDate)
                .Select(e => new
                {
                    e.Amount,
                    e.ExpenseType,
                    e.ExpenseDate,
                    e.Title,
                    e.Description
                })
                .ToListAsync(cancellationToken);

            // Calculate totals
            var totalRevenue = salesData.Sum(s => s.TotalRevenue);
            var totalCostOfGoodsSold = salesData.Sum(s => s.TotalCost);
            var grossProfit = totalRevenue - totalCostOfGoodsSold;
            var grossProfitMargin = totalRevenue > 0 ? (grossProfit / totalRevenue) * 100 : 0;

            // Group expenses by category
            var operatingExpenses = expensesData
                .GroupBy(e => e.ExpenseType.ToString())
                .Select(g => new ExpenseCategoryDto
                {
                    Category = g.Key,
                    Amount = g.Sum(e => e.Amount),
                    TransactionCount = g.Count(),
                    PercentageOfTotal = 0, // Will be calculated after total is known
                    BudgetAmount = 0, // Would need budget data
                    Variance = 0, // Would need budget data
                    VariancePercentage = 0, // Would need budget data
                    FormattedAmount = g.Sum(e => e.Amount).ToString("C")
                })
                .ToList();

            var totalOperatingExpenses = operatingExpenses.Sum(e => e.Amount);

            // Calculate percentages
            foreach (var expense in operatingExpenses)
            {
                expense.PercentageOfTotal = totalOperatingExpenses > 0 ? (expense.Amount / totalOperatingExpenses) * 100 : 0;
            }

            var operatingIncome = grossProfit - totalOperatingExpenses;
            var operatingMargin = totalRevenue > 0 ? (operatingIncome / totalRevenue) * 100 : 0;

            // Calculate other income and expenses (if applicable)
            var otherIncome = 0m; // TODO: Implement other income tracking
            var otherExpenses = 0m; // TODO: Categorize other expenses

            var netIncome = operatingIncome + otherIncome - otherExpenses;
            var netProfitMargin = totalRevenue > 0 ? (netIncome / totalRevenue) * 100 : 0;

            // Transaction counts
            var totalSalesTransactions = salesData.Count;
            var totalPurchaseTransactions = await _context.Purchases
                .Where(p => p.PurchaseDate >= request.StartDate && p.PurchaseDate <= request.EndDate)
                .CountAsync(cancellationToken);

            var averageOrderValue = totalSalesTransactions > 0 ? totalRevenue / totalSalesTransactions : 0;

            // Create result
            var result = new ProfitLossDto
            {
                PeriodStart = request.StartDate,
                PeriodEnd = request.EndDate,
                TotalRevenue = totalRevenue,
                CostOfGoodsSold = totalCostOfGoodsSold,
                GrossProfit = grossProfit,
                GrossProfitMargin = Math.Round(grossProfitMargin, 2),
                OperatingExpenses = operatingExpenses,
                TotalOperatingExpenses = totalOperatingExpenses,
                OperatingIncome = operatingIncome,
                OperatingMargin = Math.Round(operatingMargin, 2),
                OtherIncome = otherIncome,
                OtherExpenses = otherExpenses,
                NetIncome = netIncome,
                NetProfitMargin = Math.Round(netProfitMargin, 2),
                TotalSalesTransactions = totalSalesTransactions,
                TotalPurchaseTransactions = totalPurchaseTransactions,
                AverageOrderValue = Math.Round(averageOrderValue, 2),
                FormattedTotalRevenue = totalRevenue.ToString("C"),
                FormattedGrossProfit = grossProfit.ToString("C"),
                FormattedOperatingIncome = operatingIncome.ToString("C"),
                FormattedNetIncome = netIncome.ToString("C"),
                FormattedAverageOrderValue = averageOrderValue.ToString("C")
            };

            return Response.Success(result);
        }
    }
}