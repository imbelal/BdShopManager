using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Sales.Queries
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
            // Pre-aggregate sales costs
            var salesCosts = await (from od in _context.SalesItems
                                   group od by od.SalesId into g
                                   select new
                                   {
                                       SalesId = g.Key,
                                       TotalCost = g.Sum(x => x.TotalCost)
                                   }).ToListAsync(cancellationToken);

            var salesCostDict = salesCosts.ToDictionary(x => x.SalesId, x => x.TotalCost);

            // Get sales with their revenue
            var sales = await _context.Sales
                .Where(o => o.CreatedUtcDate >= request.StartDate && o.CreatedUtcDate <= request.EndDate)
                .Select(o => new
                {
                    o.Id,
                    o.CreatedUtcDate,
                    o.TotalPrice
                })
                .ToListAsync(cancellationToken);

            // Combine data in memory
            var salesWithCost = sales.Select(o => new
            {
                o.Id,
                o.CreatedUtcDate,
                o.TotalPrice,
                TotalCost = salesCostDict.ContainsKey(o.Id) ? salesCostDict[o.Id] : 0m
            });

            List<ProfitSummaryDto> summaryList;

            switch (request.GroupBy.ToLower())
            {
                case "month":
                    summaryList = salesWithCost
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
                    summaryList = salesWithCost
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
                    summaryList = salesWithCost
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
