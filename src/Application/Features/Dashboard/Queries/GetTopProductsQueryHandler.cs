using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Common.ResponseWrapper;
using Common.RequestWrapper;
using Domain.Dtos;
using Domain.Interfaces;

namespace Application.Features.Dashboard.Queries
{
    public class GetTopProductsQueryHandler : IQueryHandler<GetTopProductsQuery, List<TopProductDto>>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetTopProductsQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<List<TopProductDto>>> Handle(GetTopProductsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var startDate = DateTime.UtcNow.AddDays(-request.DaysPeriod);

                var topProducts = await _context.SalesItems
                    .Join(_context.Sales.Where(s => !s.IsDeleted && s.CreatedUtcDate >= startDate),
                          si => si.SalesId,
                          s => s.Id,
                          (si, s) => new { SalesItem = si, Sale = s })
                    .Join(_context.Products.Where(p => !p.IsDeleted),
                          x => x.SalesItem.ProductId,
                          p => p.Id,
                          (x, product) => new { x.SalesItem, x.Sale, Product = product })
                    .Join(_context.Categories.Where(c => !c.IsDeleted),
                          x => x.Product.CategoryId,
                          c => c.Id,
                          (x, category) => new { x.SalesItem, x.Sale, x.Product, Category = category })
                    .GroupBy(x => new { x.Product.Id, ProductTitle = x.Product.Title, CategoryTitle = x.Category.Title, x.Product.StockQuantity, x.Product.SellingPrice })
                    .Select(g => new TopProductDto
                    {
                        ProductId = g.Key.Id.ToString(),
                        ProductName = g.Key.ProductTitle,
                        CategoryName = g.Key.CategoryTitle,
                        ProductCode = "", // ProductCode doesn't exist on Product entity
                        TotalQuantitySold = g.Sum(x => x.SalesItem.Quantity),
                        TotalRevenue = g.Sum(x => x.SalesItem.Quantity * x.SalesItem.UnitPrice),
                        AverageUnitPrice = g.Average(x => x.SalesItem.UnitPrice),
                        SalesCount = g.Count(),
                        CurrentStock = g.Key.StockQuantity,
                        GrowthPercentage = 0, // Will be calculated below
                        ImageUrl = "", // Can be added later when product images are implemented
                        FormattedTotalRevenue = g.Sum(x => x.SalesItem.Quantity * x.SalesItem.UnitPrice).ToString("N2"),
                        FormattedAverageUnitPrice = g.Average(x => x.SalesItem.UnitPrice).ToString("N2")
                    })
                    .OrderByDescending(p => p.TotalRevenue)
                    .Take(request.Limit)
                    .ToListAsync(cancellationToken);

                // Calculate growth percentage for each product
                if (request.DaysPeriod >= 30)
                {
                    var previousPeriodStart = startDate.AddDays(-request.DaysPeriod);

                    foreach (var product in topProducts)
                    {
                        var currentPeriodRevenue = product.TotalRevenue;
                        var previousPeriodRevenue = await _context.SalesItems
                            .Join(_context.Sales.Where(s => !s.IsDeleted && s.CreatedUtcDate >= previousPeriodStart && s.CreatedUtcDate < startDate),
                                  si => si.SalesId,
                                  s => s.Id,
                                  (si, s) => new { SalesItem = si, Sale = s })
                            .Where(x => x.SalesItem.ProductId.ToString() == product.ProductId)
                            .SumAsync(x => x.SalesItem.Quantity * x.SalesItem.UnitPrice, cancellationToken);

                        product.GrowthPercentage = previousPeriodRevenue > 0 ?
                            ((currentPeriodRevenue - previousPeriodRevenue) / previousPeriodRevenue) * 100 : 0;
                    }
                }

                return Response.Success(topProducts);
            }
            catch (Exception ex)
            {
                return Response.Fail<List<TopProductDto>>($"Error retrieving top products: {ex.Message}");
            }
        }
    }
}