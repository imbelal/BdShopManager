using Common.Entities;
using Common.Entities.Interfaces;

namespace Domain.Entities
{
    public class Tenant : AuditableEntityBase<Guid>, IAggregateRoot, ISoftDeletable
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsDeleted { get; set; } = false;

        public Tenant() : base()
        {

        }

        public Tenant(string name, string address, string phoneNumber) : base(Guid.NewGuid())
        {
            Name = name;
            Address = address;
            PhoneNumber = phoneNumber;
        }
    }
}
