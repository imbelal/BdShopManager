using Common.Pagination;
using Common.RequestWrapper;
using Domain.Dtos;
using Domain.Enums;

namespace Application.Features.Purchase.Queries
{
    public class GetPurchasesQuery : IQuery<Pagination<PurchaseDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid? SupplierId { get; set; }
        public PurchaseStatus? Status { get; set; }
        public Guid? ProductId { get; set; }
        public string? SearchTerm { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}