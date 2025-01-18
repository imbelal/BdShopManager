using Common.Entities;
using Common.Entities.Interfaces;
using Domain.Dtos;
using Domain.Events;

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

        public Tenant(CreateTenantWithUserRequestDto request) : base(Guid.NewGuid())
        {
            Name = request.TenantName;
            Address = request.TenantAddress;
            PhoneNumber = request.TenantPhoneNumeber;

            RaiseDomainEvent(new TenantCreatedEvent(this.Id, request.Username, request.Password, request.FirstName, request.LastName, request.Email));
        }
    }
}
