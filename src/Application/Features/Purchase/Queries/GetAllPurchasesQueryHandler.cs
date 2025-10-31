using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Purchase.Queries
{
    public class GetAllPurchasesQueryHandler : IQueryHandler<GetAllPurchasesQuery, List<PurchaseDto>>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetAllPurchasesQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<List<PurchaseDto>>> Handle(GetAllPurchasesQuery request, CancellationToken cancellationToken)
        {
            var purchases = await _context.Purchases
                .Include(p => p.PurchaseItems)
                .OrderByDescending(p => p.PurchaseDate)
                .Select(p => new PurchaseDto
                {
                    Id = p.Id,
                    SupplierId = p.SupplierId,
                    SupplierName = _context.Suppliers
                        .Where(s => s.Id == p.SupplierId)
                        .Select(s => s.Name)
                        .FirstOrDefault(),
                    PurchaseDate = p.PurchaseDate,
                    TotalCost = p.TotalCost,
                    Remark = p.Remark,
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
                            .FirstOrDefault(),
                        Quantity = pi.Quantity,
                        CostPerUnit = pi.CostPerUnit,
                        TotalCost = pi.TotalCost
                    }).ToList()
                })
                .ToListAsync(cancellationToken);

            return Response.Success(purchases);
        }
    }
}
