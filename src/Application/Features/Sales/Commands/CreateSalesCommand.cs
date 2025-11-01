using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.Sales.Commands
{
    public class CreateSalesCommand : ICommand<Guid>
    {
        public Guid CustomerId { get; private set; }
        public decimal TotalPrice { get; private set; }
        public decimal DiscountPercentage { get; private set; }
        public decimal TotalPaid { get; private set; }
        public decimal TaxPercentage { get; private set; }
        public string Remark { get; private set; }
        public List<SalesItemDetailsDto> SalesItems { get; private set; }

        public CreateSalesCommand(Guid customerId, decimal totalPrice, decimal discountPercentage, decimal totalPaid, string remark, List<SalesItemDetailsDto> salesItemDtos, decimal taxPercentage = 0)
        {
            CustomerId = customerId;
            TotalPrice = totalPrice;
            DiscountPercentage = discountPercentage;
            TotalPaid = totalPaid;
            TaxPercentage = taxPercentage;
            Remark = remark;
            SalesItems = salesItemDtos;
        }
    }
}
