using Common.RequestWrapper;

namespace Application.Features.Tenant.Commands
{
    public class UpdateTenantCommand : ICommand<Guid>
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        public UpdateTenantCommand(string name, string address, string phoneNumber)
        {
            Name = name;
            Address = address;
            PhoneNumber = phoneNumber;
        }
    }
}