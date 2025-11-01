using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.StockTransaction.Queries
{
    public class GetStockTransactionsByProductIdQuery : IQuery<List<StockTransactionDto>>
    {
        public Guid ProductId { get; set; }
    }
}
