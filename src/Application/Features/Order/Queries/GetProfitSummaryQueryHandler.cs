using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Order.Queries
{
    public class GetProfitSummaryQueryHandler : IQueryHandler<GetProfitSummaryQuery, List<ProfitSummaryDto>>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetProfitSummaryQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<List<ProfitSummaryDto>>> Handle(GetProfitSummaryQuery request, CancellationToken cancellationToken)
        {
            // Pre-aggregate order costs
            var orderCosts = await (from od in _context.OrderDetails
                                   group od by od.OrderId into g
                                   select new
                                   {
                                       OrderId = g.Key,
                                       TotalCost = g.Sum(x => x.TotalCost)
                                   }).ToListAsync(cancellationToken);

            var orderCostDict = orderCosts.ToDictionary(x => x.OrderId, x => x.TotalCost);

            // Get orders with their revenue
            var orders = await _context.Orders
                .Where(o => o.CreatedUtcDate >= request.StartDate && o.CreatedUtcDate <= request.EndDate)
                .Select(o => new
                {
                    o.Id,
                    o.CreatedUtcDate,
                    o.TotalPrice
                })
                .ToListAsync(cancellationToken);

            // Combine data in memory
            var ordersWithCost = orders.Select(o => new
            {
                o.Id,
                o.CreatedUtcDate,
                o.TotalPrice,
                TotalCost = orderCostDict.ContainsKey(o.Id) ? orderCostDict[o.Id] : 0m
            });

            List<ProfitSummaryDto> summaryList;

            switch (request.GroupBy.ToLower())
            {
                case "month":
                    summaryList = ordersWithCost
                        .GroupBy(o => new { o.CreatedUtcDate.Year, o.CreatedUtcDate.Month })
                        .Select(g => new ProfitSummaryDto
                        {
                            Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                            TotalOrders = g.Count(),
                            TotalRevenue = g.Sum(o => o.TotalPrice),
                            TotalCost = g.Sum(o => o.TotalCost),
                            TotalProfit = g.Sum(o => o.TotalPrice) - g.Sum(o => o.TotalCost),
                            AverageProfitMargin = g.Sum(o => o.TotalPrice) > 0
                                ? Math.Round(((g.Sum(o => o.TotalPrice) - g.Sum(o => o.TotalCost)) / g.Sum(o => o.TotalPrice)) * 100, 2)
                                : 0
                        })
                        .OrderBy(s => s.Date)
                        .ToList();
                    break;

                case "year":
                    summaryList = ordersWithCost
                        .GroupBy(o => o.CreatedUtcDate.Year)
                        .Select(g => new ProfitSummaryDto
                        {
                            Date = new DateTime(g.Key, 1, 1),
                            TotalOrders = g.Count(),
                            TotalRevenue = g.Sum(o => o.TotalPrice),
                            TotalCost = g.Sum(o => o.TotalCost),
                            TotalProfit = g.Sum(o => o.TotalPrice) - g.Sum(o => o.TotalCost),
                            AverageProfitMargin = g.Sum(o => o.TotalPrice) > 0
                                ? Math.Round(((g.Sum(o => o.TotalPrice) - g.Sum(o => o.TotalCost)) / g.Sum(o => o.TotalPrice)) * 100, 2)
                                : 0
                        })
                        .OrderBy(s => s.Date)
                        .ToList();
                    break;

                case "day":
                default:
                    summaryList = ordersWithCost
                        .GroupBy(o => new { o.CreatedUtcDate.Year, o.CreatedUtcDate.Month, o.CreatedUtcDate.Day })
                        .Select(g => new ProfitSummaryDto
                        {
                            Date = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                            TotalOrders = g.Count(),
                            TotalRevenue = g.Sum(o => o.TotalPrice),
                            TotalCost = g.Sum(o => o.TotalCost),
                            TotalProfit = g.Sum(o => o.TotalPrice) - g.Sum(o => o.TotalCost),
                            AverageProfitMargin = g.Sum(o => o.TotalPrice) > 0
                                ? Math.Round(((g.Sum(o => o.TotalPrice) - g.Sum(o => o.TotalCost)) / g.Sum(o => o.TotalPrice)) * 100, 2)
                                : 0
                        })
                        .OrderBy(s => s.Date)
                        .ToList();
                    break;
            }

            return Response.Success(summaryList);
        }
    }
}
