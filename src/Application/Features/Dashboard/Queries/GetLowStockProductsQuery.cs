using Common.RequestWrapper;
using Domain.Dtos;
using System.Collections.Generic;

namespace Application.Features.Dashboard.Queries
{
    public class GetLowStockProductsQuery : IQuery<List<LowStockProductDto>>
    {
        public int Limit { get; set; } = 20;
        public bool IncludeOutOfStock { get; set; } = true;

        public GetLowStockProductsQuery()
        {
        }

        public GetLowStockProductsQuery(int limit, bool includeOutOfStock = true)
        {
            Limit = limit > 0 ? limit : 20;
            IncludeOutOfStock = includeOutOfStock;
        }
    }
}