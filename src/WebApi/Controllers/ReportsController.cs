using Application.Features.Reports.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get profit and loss statement for a specific date range
        /// </summary>
        /// <param name="startDate">Start date for the report period</param>
        /// <param name="endDate">End date for the report period</param>
        /// <param name="groupBy">Grouping period: day, month, or year (default: month)</param>
        /// <param name="includeComparisons">Include previous period comparisons (default: true)</param>
        /// <param name="currency">Currency code (default: USD)</param>
        /// <returns>Profit and loss statement data</returns>
        [HttpGet("financial/profit-loss")]
        public async Task<IActionResult> GetProfitLoss(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string groupBy = "month",
            [FromQuery] bool includeComparisons = true,
            [FromQuery] string currency = "USD")
        {
            var query = new GetProfitLossQuery(startDate, endDate, groupBy, includeComparisons)
            {
                Currency = currency
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get revenue breakdown analysis for a specific date range
        /// </summary>
        /// <param name="startDate">Start date for the report period</param>
        /// <param name="endDate">End date for the report period</param>
        /// <param name="groupBy">Grouping period: day, month, or year (default: month)</param>
        /// <param name="includeTopProducts">Include top selling products (default: true)</param>
        /// <param name="includeCustomerBreakdown">Include customer revenue breakdown (default: true)</param>
        /// <param name="topProductsCount">Number of top products to include (default: 10)</param>
        /// <param name="topCustomersCount">Number of top customers to include (default: 10)</param>
        /// <param name="currency">Currency code (default: USD)</param>
        /// <returns>Revenue breakdown analysis data</returns>
        [HttpGet("financial/revenue-breakdown")]
        public async Task<IActionResult> GetRevenueBreakdown(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string groupBy = "month",
            [FromQuery] bool includeTopProducts = true,
            [FromQuery] bool includeCustomerBreakdown = true,
            [FromQuery] int topProductsCount = 10,
            [FromQuery] int topCustomersCount = 10,
            [FromQuery] string currency = "USD")
        {
            var query = new GetRevenueBreakdownQuery(startDate, endDate, groupBy,
                   includeTopProducts, includeCustomerBreakdown, topProductsCount, topCustomersCount)
            {
                Currency = currency
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get expense analysis for a specific date range
        /// </summary>
        /// <param name="startDate">Start date for the report period</param>
        /// <param name="endDate">End date for the report period</param>
        /// <param name="groupBy">Grouping period: day, month, or year (default: month)</param>
        /// <param name="includeVendorBreakdown">Include vendor/supplier breakdown (default: true)</param>
        /// <param name="includeBudgetComparison">Include budget vs actual comparison (default: true)</param>
        /// <param name="topVendorsCount">Number of top vendors to include (default: 10)</param>
        /// <param name="currency">Currency code (default: USD)</param>
        /// <returns>Expense analysis data</returns>
        [HttpGet("financial/expense-analysis")]
        public async Task<IActionResult> GetExpenseAnalysis(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string groupBy = "month",
            [FromQuery] bool includeVendorBreakdown = true,
            [FromQuery] bool includeBudgetComparison = true,
            [FromQuery] int topVendorsCount = 10,
            [FromQuery] string currency = "USD")
        {

            var query = new GetExpenseAnalysisQuery(startDate, endDate, groupBy,
                includeVendorBreakdown, includeBudgetComparison, topVendorsCount)
            {
                Currency = currency
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get comprehensive financial metrics and KPIs for a specific date range
        /// </summary>
        /// <param name="startDate">Start date for the report period</param>
        /// <param name="endDate">End date for the report period</param>
        /// <param name="includeKPIs">Include key performance indicators (default: true)</param>
        /// <param name="includeRatios">Include financial ratios (default: true)</param>
        /// <param name="includeCashFlow">Include cash flow analysis (default: true)</param>
        /// <param name="currency">Currency code (default: USD)</param>
        /// <returns>Financial metrics and KPIs data</returns>
        [HttpGet("financial/metrics")]
        public async Task<IActionResult> GetFinancialMetrics(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] bool includeKPIs = true,
            [FromQuery] bool includeRatios = true,
            [FromQuery] bool includeCashFlow = true,
            [FromQuery] string currency = "USD")
        {
            var query = new GetFinancialMetricsQuery(startDate, endDate,
                    includeKPIs, includeRatios, includeCashFlow)
            {
                Currency = currency
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get a summary of all financial reports for dashboard view
        /// </summary>
        /// <param name="startDate">Start date for the report period (default: start of current month)</param>
        /// <param name="endDate">End date for the report period (default: today)</param>
        /// <param name="currency">Currency code (default: USD)</param>
        /// <returns>Financial summary data for dashboard</returns>
        [HttpGet("financial/summary")]
        public async Task<IActionResult> GetFinancialSummary(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string currency = "USD")
        {
            // Set default dates if not provided
            var start = startDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var end = endDate ?? DateTime.Now;

            // Get profit loss summary
            var profitLossQuery = new GetProfitLossQuery(start, end, "month", true)
            {
                Currency = currency
            };
            var profitLossResult = await _mediator.Send(profitLossQuery);

            // Get financial metrics
            var metricsQuery = new GetFinancialMetricsQuery(start, end, true, true, true)
            {
                Currency = currency
            };
            var metricsResult = await _mediator.Send(metricsQuery);

            var summary = new
            {
                Period = new { Start = start, End = end },
                Currency = currency,
                ProfitLoss = profitLossResult.Data,
                FinancialMetrics = metricsResult.Data,
                GeneratedAt = DateTime.Now
            };

            return Ok(summary);
        }
    }
}