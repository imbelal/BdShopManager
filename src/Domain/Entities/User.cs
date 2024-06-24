using Common.Entities;
using Common.Entities.Interfaces;

namespace Domain.Entities
{
    public class User : AuditableEntityBase, IAggregateRoot, ISoftDeletable
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid UserRoleId { get; set; }
        public UserRole UserRole { get; set; }
        public bool IsDeleted { get; set; }

        public User() { } // Needed for ef core

        public User(string username, string passwordHash, string email, string firstname, string lastname, Guid userRoleId)
        {
            Username = username;
            PasswordHash = passwordHash;
            Email = email;
            FirstName = firstname;
            LastName = lastname;
            UserRoleId = userRoleId;
        }

        public void UpdateProfileInfo(string email, string firstname, string lastname)
        {
            Email = email.Trim();
            FirstName = firstname.Trim();
            LastName = lastname.Trim();
        }

        public void ChangePassword(string oldPasswordHash, string newPasswordHash)
        {
            if (PasswordHash == oldPasswordHash)
                PasswordHash = newPasswordHash;
        }
    }
}
