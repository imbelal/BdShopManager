using Common.Entities;
using Common.Entities.Interfaces;

namespace Domain.Entities
{
    public class Supplier : AuditableEntityBase, IAggregateRoot, ISoftDeletable
    {
        public string Name { get; private set; }
        public string Details { get; private set; }
        public string ContactNo { get; private set; }
        public bool IsDeleted { get; set; } = false;

        public Supplier(string name, string details, string contactNo)
        {
            Name = name;
            Details = details;
            ContactNo = contactNo;
        }

        public void Update(string name, string details, string contactNo)
        {
            Name = name;
            Details = details;
            ContactNo = contactNo;
        }
    }
}
