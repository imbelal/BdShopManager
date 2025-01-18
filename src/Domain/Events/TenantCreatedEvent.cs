using Common.Events;

namespace Domain.Events
{
    public class TenantCreatedEvent : IDomainEvent
    {
        public Guid TenantId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public TenantCreatedEvent(Guid tenantId, string username, string password, string firstName, string lastName, string email)
        {
            TenantId = tenantId;
            Username = username;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }
    }
}
