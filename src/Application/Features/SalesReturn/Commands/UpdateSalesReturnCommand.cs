using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.SalesReturn.Commands
{
    public class UpdateSalesReturnCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }
        public decimal TotalRefundAmount { get; set; }
        public string Remark { get; set; }
        public List<UpdateSalesReturnItemDto> SalesReturnItems { get; set; } = new();
    }
}