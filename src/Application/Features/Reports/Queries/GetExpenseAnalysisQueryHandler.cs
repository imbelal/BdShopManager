using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Reports.Queries
{
    public class GetExpenseAnalysisQueryHandler : IQueryHandler<GetExpenseAnalysisQuery, ExpenseAnalysisDto>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetExpenseAnalysisQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<ExpenseAnalysisDto>> Handle(GetExpenseAnalysisQuery request, CancellationToken cancellationToken)
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

            var baseQuery = _context.Expenses
                .Where(e => e.ExpenseDate >= request.StartDate && e.ExpenseDate <= request.EndDate);

            // Get total expenses
            var totalExpenses = await baseQuery.SumAsync(e => e.Amount, cancellationToken);
            var totalTransactions = await baseQuery.CountAsync(cancellationToken);

            // Category breakdown
            var categoryBreakdown = await baseQuery
                .GroupBy(e => e.ExpenseType)
                .Select(g => new ExpenseCategoryBreakdownDto
                {
                    CategoryType = g.Key.ToString(),
                    TotalAmount = g.Sum(e => e.Amount),
                    TransactionCount = g.Count(),
                    AverageTransactionAmount = g.Average(e => e.Amount),
                    PercentageOfTotal = 0, // Will be calculated after total is known
                    BudgetAmount = 0, // Would need budget data
                    Variance = 0, // Would need budget data
                    VariancePercentage = 0, // Would need budget data
                    FormattedTotalAmount = g.Sum(e => e.Amount).ToString("C"),
                    FormattedAverageTransactionAmount = g.Average(e => e.Amount).ToString("C"),
                    FormattedBudgetAmount = "0",
                    FormattedVariance = "0"
                })
                .ToListAsync(cancellationToken);

            // Calculate percentages
            foreach (var category in categoryBreakdown)
            {
                category.PercentageOfTotal = totalExpenses > 0 ? (category.TotalAmount / totalExpenses) * 100 : 0;
            }

            // Monthly trends
            var monthlyTrends = new List<ExpenseTrendDto>();
            var currentDate = new DateTime(request.StartDate.Year, request.StartDate.Month, 1);

            while (currentDate <= request.EndDate)
            {
                var periodEnd = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
                periodEnd = periodEnd > request.EndDate ? request.EndDate : periodEnd;

                var periodExpenses = await baseQuery
                    .Where(e => e.ExpenseDate >= currentDate && e.ExpenseDate <= periodEnd)
                    .SumAsync(e => e.Amount, cancellationToken);

                var periodTransactions = await baseQuery
                    .Where(e => e.ExpenseDate >= currentDate && e.ExpenseDate <= periodEnd)
                    .CountAsync(cancellationToken);

                var avgTransactionAmount = periodTransactions > 0 ? periodExpenses / periodTransactions : 0;

                monthlyTrends.Add(new ExpenseTrendDto
                {
                    Period = currentDate,
                    TotalAmount = periodExpenses,
                    TransactionCount = periodTransactions,
                    AverageTransactionAmount = avgTransactionAmount,
                    GrowthRate = 0, // Would need previous period data
                    FormattedPeriod = currentDate.ToString("MMM yyyy"),
                    FormattedTotalAmount = periodExpenses.ToString("C"),
                    FormattedAverageTransactionAmount = avgTransactionAmount.ToString("C"),
                    FormattedGrowthRate = "0%"
                });

                currentDate = currentDate.AddMonths(1);
            }

            // Vendor breakdown (if expense has supplier/vendor information)
            var vendorBreakdown = new List<ExpenseByVendorDto>();
            if (request.IncludeVendorBreakdown)
            {
                // Note: This assumes expenses might have vendor information
                // If not, this would need to be adapted based on the actual data model
                var expensesWithRemarks = await baseQuery
                    .Where(e => !string.IsNullOrEmpty(e.Remarks)) // Using remarks as proxy for vendor
                    .Select(e => new
                    {
                        e.Amount,
                        e.ExpenseType,
                        TruncatedRemarks = e.Remarks.Length > 50 ? e.Remarks.Substring(0, 50) : e.Remarks
                    })
                    .ToListAsync(cancellationToken);

                vendorBreakdown = expensesWithRemarks
                    .GroupBy(e => e.TruncatedRemarks) // First 50 chars as vendor name
                    .Select(g => new ExpenseByVendorDto
                    {
                        VendorId = g.Key.GetHashCode().ToString(), // Generate a hash-based ID
                        VendorName = g.Key,
                        TotalAmount = g.Sum(e => e.Amount),
                        TransactionCount = g.Count(),
                        AverageTransactionAmount = g.Average(e => e.Amount),
                        CategoryType = g.FirstOrDefault().ExpenseType.ToString(),
                        PercentageOfTotal = totalExpenses > 0 ? (g.Sum(e => e.Amount) / totalExpenses) * 100 : 0,
                        FormattedTotalAmount = g.Sum(e => e.Amount).ToString("C"),
                        FormattedAverageTransactionAmount = g.Average(e => e.Amount).ToString("C")
                    })
                    .OrderByDescending(v => v.TotalAmount)
                    .Take(request.TopVendorsCount)
                    .ToList();
            }

            // Previous period expenses for growth calculation
            var previousPeriodStart = request.StartDate.AddMonths(-1);
            var previousPeriodEnd = request.StartDate.AddDays(-1);
            var previousPeriodExpenses = await _context.Expenses
                .Where(e => e.ExpenseDate >= previousPeriodStart && e.ExpenseDate <= previousPeriodEnd)
                .SumAsync(e => e.Amount, cancellationToken);

            var growthRate = previousPeriodExpenses > 0 ?
                ((totalExpenses - previousPeriodExpenses) / previousPeriodExpenses) * 100 : 0;

            var averageExpensePerTransaction = totalTransactions > 0 ? totalExpenses / totalTransactions : 0;

            // Budget comparison (placeholder values - would need actual budget data)
            var budgetAmount = totalExpenses * 1.1m; // Assuming 10% over budget
            var budgetVsActual = budgetAmount - totalExpenses;
            var budgetUtilizationPercentage = budgetAmount > 0 ? (totalExpenses / budgetAmount) * 100 : 0;

            var result = new ExpenseAnalysisDto
            {
                PeriodStart = request.StartDate,
                PeriodEnd = request.EndDate,
                TotalExpenses = totalExpenses,
                CategoryBreakdown = categoryBreakdown,
                MonthlyTrends = monthlyTrends,
                VendorBreakdown = vendorBreakdown,
                PreviousPeriodExpenses = previousPeriodExpenses,
                GrowthRate = Math.Round(growthRate, 2),
                AverageExpensePerTransaction = Math.Round(averageExpensePerTransaction, 2),
                BudgetVsActual = Math.Round(budgetVsActual, 2),
                BudgetUtilizationPercentage = Math.Round(budgetUtilizationPercentage, 2),
                FormattedTotalExpenses = totalExpenses.ToString("C"),
                FormattedGrowthRate = $"{Math.Round(growthRate, 2)}%",
                FormattedAverageExpensePerTransaction = averageExpensePerTransaction.ToString("C"),
                FormattedBudgetVsActual = budgetVsActual.ToString("C")
            };

            return Response.Success(result);
        }
    }
}