using Common.RequestWrapper;

namespace Application.Features.Sales.Commands
{
    public class AddPaymentCommand : ICommand<bool>
    {
        public Guid SalesId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string Remark { get; set; } = string.Empty;
    }
}
