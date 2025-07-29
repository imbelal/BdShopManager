using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Inventory.Queries
{
    public class GetAllInventoriesQueryHandler : IQueryHandler<GetAllInventoriesQuery, List<InventoryDto>>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetAllInventoriesQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<List<InventoryDto>>> Handle(GetAllInventoriesQuery request, CancellationToken cancellationToken)
        {
            var inventories = await _context.Inventories
                .Where(i => !i.IsDeleted)
                .Join(_context.Products, i => i.ProductId, p => p.Id, (i, p) => new { Inventory = i, Product = p })
                .Join(_context.Suppliers, ip => ip.Inventory.SupplierId, s => s.Id, (ip, s) => new { ip.Inventory, ip.Product, Supplier = s })
                .OrderByDescending(x => x.Inventory.CreatedUtcDate)
                .Select(x => new InventoryDto
                {
                    Id = x.Inventory.Id,
                    ProductId = x.Inventory.ProductId,
                    ProductName = x.Product.Title,
                    SupplierId = x.Inventory.SupplierId,
                    SupplierName = x.Supplier.Name,
                    Quantity = x.Inventory.Quantity,
                    CostPerUnit = x.Inventory.CostPerUnit,
                    TotalCost = x.Inventory.TotalCost,
                    Remark = x.Inventory.Remark ?? string.Empty,
                    CreatedBy = x.Inventory.CreatedBy,
                    CreatedDate = x.Inventory.CreatedUtcDate.ToString("yyyy-MM-dd HH:mm:ss")
                })
                .ToListAsync(cancellationToken);

            if (inventories.Count == 0)
                return Response.Fail<List<InventoryDto>>("No inventories found!");

            return Response.Success(inventories);
        }
    }
}
