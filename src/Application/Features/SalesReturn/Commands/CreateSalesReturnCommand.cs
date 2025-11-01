using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.SalesReturn.Commands
{
    public class CreateSalesReturnCommand : ICommand<SalesReturnDto>
    {
        public Guid SalesId { get; private set; }
        public decimal TotalRefundAmount { get; private set; }
        public string Remark { get; private set; }
        public List<SalesReturnItemDetailsDto> SalesReturnItems { get; private set; }

        public CreateSalesReturnCommand(Guid salesId, decimal totalRefundAmount, string remark, List<SalesReturnItemDetailsDto> salesReturnItems)
        {
            SalesId = salesId;
            TotalRefundAmount = totalRefundAmount;
            Remark = remark;
            SalesReturnItems = salesReturnItems;
        }
    }
}
