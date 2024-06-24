
using Common.Entities;
using Common.Entities.Interfaces;

namespace Domain.Entities
{
    public class UserRole : AuditableEntityBase, IAggregateRoot
    {
        public string Title { get; set; }
    }
}
