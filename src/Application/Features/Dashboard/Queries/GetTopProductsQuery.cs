using Common.RequestWrapper;
using Domain.Dtos;
using System.Collections.Generic;

namespace Application.Features.Dashboard.Queries
{
    public class GetTopProductsQuery : IQuery<List<TopProductDto>>
    {
        public int Limit { get; set; } = 10;
        public int DaysPeriod { get; set; } = 30;

        public GetTopProductsQuery()
        {
        }

        public GetTopProductsQuery(int limit, int daysPeriod = 30)
        {
            Limit = limit > 0 ? limit : 10;
            DaysPeriod = daysPeriod > 0 ? daysPeriod : 30;
        }
    }
}