using Common.Pagination;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Product.Queries
{
    public class GetProductProfitabilityQueryHandler : IQueryHandler<GetProductProfitabilityQuery, Pagination<ProductProfitabilityDto>>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetProductProfitabilityQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<Pagination<ProductProfitabilityDto>>> Handle(GetProductProfitabilityQuery request, CancellationToken cancellationToken)
        {
            // Start with all sales items
            var salesItemsQuery = _context.SalesItems.AsQueryable();

            // Apply date range filter if provided
            if (request.StartDate.HasValue)
            {
                salesItemsQuery = salesItemsQuery.Where(si =>
                    _context.Sales.Any(s => s.Id == si.SalesId && s.CreatedUtcDate >= request.StartDate.Value));
            }

            if (request.EndDate.HasValue)
            {
                salesItemsQuery = salesItemsQuery.Where(si =>
                    _context.Sales.Any(s => s.Id == si.SalesId && s.CreatedUtcDate <= request.EndDate.Value));
            }

            // Group by product and calculate metrics
            var profitabilityQuery = from si in salesItemsQuery
                                     group si by si.ProductId into g
                                     select new
                                     {
                                         ProductId = g.Key,
                                         TotalUnitsSold = g.Sum(x => x.Quantity),
                                         TotalRevenue = g.Sum(x => x.TotalPrice),
                                         TotalCost = g.Sum(x => x.TotalCost)
                                     } into grouped
                                     join p in _context.Products on grouped.ProductId equals p.Id into productGroup
                                     from product in productGroup.DefaultIfEmpty()
                                     let totalProfit = grouped.TotalRevenue - grouped.TotalCost
                                     let avgProfitMargin = grouped.TotalRevenue > 0
                                         ? Math.Round((totalProfit / grouped.TotalRevenue) * 100, 2)
                                         : 0
                                     let currentProfitMargin = product != null && product.SellingPrice > 0
                                         ? Math.Round(((product.SellingPrice - product.CostPrice) / product.SellingPrice) * 100, 2)
                                         : 0
                                     select new ProductProfitabilityDto
                                     {
                                         ProductId = grouped.ProductId,
                                         ProductTitle = product != null ? product.Title : "Unknown",
                                         TotalUnitsSold = grouped.TotalUnitsSold,
                                         TotalRevenue = grouped.TotalRevenue,
                                         TotalCost = grouped.TotalCost,
                                         TotalProfit = totalProfit,
                                         AverageProfitMargin = avgProfitMargin,
                                         CurrentCostPrice = product != null ? product.CostPrice : 0,
                                         CurrentSellingPrice = product != null ? product.SellingPrice : 0,
                                         CurrentProfitMargin = currentProfitMargin
                                     };

            // Apply profit margin filter if provided
            if (request.MinimumProfitMargin.HasValue)
            {
                profitabilityQuery = profitabilityQuery.Where(p => p.AverageProfitMargin >= request.MinimumProfitMargin.Value);
            }

            // Order by total profit descending
            profitabilityQuery = profitabilityQuery.OrderByDescending(p => p.TotalProfit);

            var pagination = await new Pagination<ProductProfitabilityDto>()
                .CreateAsync(profitabilityQuery, request.PageNumber, request.PageSize, cancellationToken);

            return Response.Success(pagination);
        }
    }
}
