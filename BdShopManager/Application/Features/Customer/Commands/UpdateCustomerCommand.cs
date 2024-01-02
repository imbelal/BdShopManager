using Common.RequestWrapper;

namespace Application.Features.Customer.Commands
{
    public class UpdateCustomerCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string Remark { get; set; }
    }
}
