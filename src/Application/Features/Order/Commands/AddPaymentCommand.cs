using Common.RequestWrapper;

namespace Application.Features.Order.Commands
{
    public class AddPaymentCommand : ICommand<bool>
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string Remark { get; set; } = string.Empty;
    }
}
