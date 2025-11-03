using Application.Features.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get comprehensive dashboard metrics and KPIs
        /// </summary>
        /// <param name="startDate">Optional start date (defaults to current month start)</param>
        /// <param name="endDate">Optional end date (defaults to now)</param>
        /// <returns>Dashboard metrics including revenue, sales, purchases, inventory, and customer KPIs</returns>
        [HttpGet("metrics")]
        public async Task<IActionResult> GetDashboardMetrics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var query = new GetDashboardMetricsQuery(startDate, endDate);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Get recent sales for dashboard display
        /// </summary>
        /// <param name="limit">Maximum number of sales to return (default: 10)</param>
        /// <param name="pageNumber">Page number for pagination (default: 1)</param>
        /// <param name="pageSize">Page size for pagination (default: 10)</param>
        /// <returns>Paginated list of recent sales</returns>
        [HttpGet("recent-sales")]
        public async Task<IActionResult> GetRecentSales([FromQuery] int limit = 10, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetRecentSalesQuery(limit)
            {
                PageNumber = pageNumber,
                PageSize = pageSize > 0 ? pageSize : limit
            };

            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Get top performing products by revenue
        /// </summary>
        /// <param name="limit">Maximum number of products to return (default: 10)</param>
        /// <param name="daysPeriod">Number of days to consider for performance analysis (default: 30)</param>
        /// <returns>List of top performing products</returns>
        [HttpGet("top-products")]
        public async Task<IActionResult> GetTopProducts([FromQuery] int limit = 10, [FromQuery] int daysPeriod = 30)
        {
            var query = new GetTopProductsQuery(limit, daysPeriod);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Get products with low stock levels
        /// </summary>
        /// <param name="limit">Maximum number of products to return (default: 20)</param>
        /// <param name="includeOutOfStock">Whether to include completely out of stock products (default: true)</param>
        /// <returns>List of products that need to be reordered</returns>
        [HttpGet("low-stock-products")]
        public async Task<IActionResult> GetLowStockProducts([FromQuery] int limit = 20, [FromQuery] bool includeOutOfStock = true)
        {
            var query = new GetLowStockProductsQuery(limit, includeOutOfStock);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get sales trends over time
        /// </summary>
        /// <param name="days">Number of days to analyze (default: 30)</param>
        /// <returns>Sales trend data including daily and monthly breakdowns</returns>
        [HttpGet("sales-trends")]
        public async Task<IActionResult> GetSalesTrends([FromQuery] int days = 30)
        {
            // For now, return a placeholder - this will be implemented in a future phase
            return Ok(new { message = "Sales trends endpoint coming in next phase" });
        }
    }
}