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
    public class GetLowStockProductsQueryHandler : IQueryHandler<GetLowStockProductsQuery, List<LowStockProductDto>>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetLowStockProductsQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<List<LowStockProductDto>>> Handle(GetLowStockProductsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var lastMonthStart = DateTime.UtcNow.AddDays(-30);

                // Note: MinimumStock doesn't exist on Product entity, using a default threshold of 10
                var lowStockProductsQuery = _context.Products
                    .Where(p => !p.IsDeleted && p.StockQuantity <= 10);

                if (!request.IncludeOutOfStock)
                {
                    lowStockProductsQuery = lowStockProductsQuery.Where(p => p.StockQuantity > 0);
                }

                var lowStockProducts = await lowStockProductsQuery
                    .Join(_context.Categories.Where(c => !c.IsDeleted),
                          p => p.CategoryId,
                          c => c.Id,
                          (product, category) => new { Product = product, Category = category })
                    .Select(x => new LowStockProductDto
                    {
                        ProductId = x.Product.Id.ToString(),
                        ProductName = x.Product.Title,
                        CategoryName = x.Category.Title,
                        ProductCode = "", // ProductCode doesn't exist on Product entity
                        CurrentStock = x.Product.StockQuantity,
                        MinimumStock = 10, // Default threshold since it doesn't exist on Product entity
                        ReorderLevel = 5,  // Default threshold since it doesn't exist on Product entity
                        UnitCost = x.Product.CostPrice,
                        UnitPrice = x.Product.SellingPrice,
                        LastMonthQuantitySold = 0, // Will be calculated below
                        IsOutOfStock = x.Product.StockQuantity == 0,
                        Urgency = "", // Will be calculated below
                        FormattedUnitCost = x.Product.CostPrice.ToString("N2"),
                        FormattedUnitPrice = x.Product.SellingPrice.ToString("N2")
                    })
                    .OrderBy(p => p.CurrentStock)
                    .Take(request.Limit)
                    .ToListAsync(cancellationToken);

                // Calculate last month sales for each product
                var productIds = lowStockProducts.Select(p => p.ProductId).ToList();

                var lastMonthSales = await _context.SalesItems
                    .Join(_context.Sales.Where(s => !s.IsDeleted && s.CreatedUtcDate >= lastMonthStart),
                          si => si.SalesId,
                          s => s.Id,
                          (si, s) => new { SalesItem = si, Sale = s })
                    .Where(x => productIds.Contains(x.SalesItem.ProductId.ToString()))
                    .GroupBy(x => x.SalesItem.ProductId.ToString())
                    .Select(g => new { ProductId = g.Key, TotalQuantity = g.Sum(x => x.SalesItem.Quantity) })
                    .ToListAsync(cancellationToken);

                // Update last month sales and calculate urgency
                foreach (var product in lowStockProducts)
                {
                    var lastMonthSale = lastMonthSales.FirstOrDefault(s => s.ProductId == product.ProductId);
                    product.LastMonthQuantitySold = lastMonthSale?.TotalQuantity ?? 0;

                    // Calculate urgency based on current stock vs last month sales
                    if (product.IsOutOfStock)
                    {
                        product.Urgency = "Critical - Out of Stock";
                    }
                    else if (product.CurrentStock == 0)
                    {
                        product.Urgency = "Critical - No Stock";
                    }
                    else if (product.LastMonthQuantitySold > 0 && product.CurrentStock < product.LastMonthQuantitySold)
                    {
                        product.Urgency = "High - Won't Last Month";
                    }
                    else if (product.CurrentStock <= product.ReorderLevel)
                    {
                        product.Urgency = "Medium - Reorder Point";
                    }
                    else
                    {
                        product.Urgency = "Low - Monitor";
                    }
                }

                // Sort by urgency and then by stock level
                var urgencyOrder = new Dictionary<string, int>
                {
                    ["Critical - Out of Stock"] = 1,
                    ["Critical - No Stock"] = 2,
                    ["High - Won't Last Month"] = 3,
                    ["Medium - Reorder Point"] = 4,
                    ["Low - Monitor"] = 5
                };

                lowStockProducts = lowStockProducts
                    .OrderBy(p => urgencyOrder.GetValueOrDefault(p.Urgency, 99))
                    .ThenBy(p => p.CurrentStock)
                    .ToList();

                return Response.Success(lowStockProducts);
            }
            catch (Exception ex)
            {
                return Response.Fail<List<LowStockProductDto>>($"Error retrieving low stock products: {ex.Message}");
            }
        }
    }
}