using Common.RequestWrapper;
using Domain.Dtos;
using Domain.Enums;

namespace Application.Features.StockTransaction.Queries
{
    public class GetAllStockTransactionsQuery : IQuery<List<StockTransactionDto>>
    {
        public Guid? ProductId { get; set; }
        public StockTransactionType? Type { get; set; }
        public StockReferenceType? RefType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
