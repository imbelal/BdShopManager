using Common.RequestWrapper;

namespace Application.Features.Supplier.Commands
{
    public class UpdateSupplierCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public string ContactNo { get; set; }

        public UpdateSupplierCommand(Guid id, string name, string details, string contactNo)
        {
            Id = id;
            Name = name;
            Details = details;
            ContactNo = contactNo;
        }
    }
}
