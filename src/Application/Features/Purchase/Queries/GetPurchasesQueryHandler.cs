using Common.Pagination;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Purchase.Queries
{
    public class GetPurchasesQueryHandler : IQueryHandler<GetPurchasesQuery, Pagination<PurchaseDto>>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetPurchasesQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<Pagination<PurchaseDto>>> Handle(GetPurchasesQuery request, CancellationToken cancellationToken)
        {
            var purchasesQuery = _context.Purchases
                .Include(p => p.PurchaseItems)
                .AsQueryable();

            // Apply filters
            if (request.SupplierId.HasValue)
            {
                purchasesQuery = purchasesQuery.Where(p => p.SupplierId == request.SupplierId.Value);
            }

            if (request.Status.HasValue)
            {
                purchasesQuery = purchasesQuery.Where(p => p.Status == request.Status.Value);
            }

            if (request.ProductId.HasValue)
            {
                purchasesQuery = purchasesQuery.Where(p => p.PurchaseItems.Any(pi => pi.ProductId == request.ProductId.Value));
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                purchasesQuery = purchasesQuery.Where(p =>
                    p.Remark.ToLower().Contains(searchTerm) ||
                    _context.Suppliers
                        .Where(s => s.Id == p.SupplierId)
                        .Any(s => s.Name.ToLower().Contains(searchTerm)));
            }

            if (request.StartDate.HasValue)
            {
                purchasesQuery = purchasesQuery.Where(p => p.PurchaseDate >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                purchasesQuery = purchasesQuery.Where(p => p.PurchaseDate <= request.EndDate.Value);
            }

            var purchasesDtoQuery = purchasesQuery
                .OrderByDescending(p => p.PurchaseDate)
                .Select(p => new PurchaseDto
                {
                    Id = p.Id,
                    SupplierId = p.SupplierId,
                    SupplierName = _context.Suppliers
                        .Where(s => s.Id == p.SupplierId)
                        .Select(s => s.Name)
                        .FirstOrDefault() ?? "",
                    PurchaseDate = p.PurchaseDate,
                    TotalCost = p.TotalCost,
                    Remark = p.Remark,
                    Status = p.Status,
                    CreatedBy = p.CreatedBy,
                    CreatedDate = p.CreatedUtcDate.ToString(),
                    PurchaseItems = p.PurchaseItems.Select(pi => new PurchaseItemDto
                    {
                        Id = pi.Id,
                        PurchaseId = pi.PurchaseId,
                        ProductId = pi.ProductId,
                        ProductName = _context.Products
                            .Where(prod => prod.Id == pi.ProductId)
                            .Select(prod => prod.Title)
                            .FirstOrDefault() ?? "",
                        Quantity = pi.Quantity,
                        CostPerUnit = pi.CostPerUnit,
                        TotalCost = pi.TotalCost
                    }).ToList()
                });

            var pagination = await new Pagination<PurchaseDto>().CreateAsync(purchasesDtoQuery, request.PageNumber, request.PageSize, cancellationToken);

            return Response.Success(pagination);
        }
    }
}