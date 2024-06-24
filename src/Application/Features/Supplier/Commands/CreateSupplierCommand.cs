using Common.RequestWrapper;

namespace Application.Features.Supplier.Commands
{
    public class CreateSupplierCommand : ICommand<Guid>
    {
        public string Name { get; set; }
        public string Details { get; set; }
        public string ContactNo { get; set; }

        public CreateSupplierCommand(string name, string details, string contactNo)
        {
            Name = name;
            Details = details;
            ContactNo = contactNo;
        }
    }
}
