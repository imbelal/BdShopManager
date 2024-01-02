using Common.Entities;
using Common.Entities.Interfaces;

namespace Domain.Entities
{
    public class Customer : AuditableEntityBase, IAggregateRoot, ISoftDeletable
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Address { get; private set; }
        public string ContactNo { get; private set; }
        public string Email { get; private set; }
        public string Remark { get; private set; }
        public bool IsDeleted { get; set; } = false;

        public Customer(string firstName, string lastName, string address, string contactNo, string email, string remark)
        {
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            ContactNo = contactNo;
            Email = email;
            Remark = remark;
        }

        public void Update(string firstName, string lastName, string address, string contactNo, string email, string remark)
        {
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            ContactNo = contactNo;
            Email = email;
            Remark = remark;
        }
    }
}
