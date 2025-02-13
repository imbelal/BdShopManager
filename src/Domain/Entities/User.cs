﻿using Common.Entities;
using Common.Entities.Interfaces;

namespace Domain.Entities
{
    public class User : AuditableTenantEntityBase<Guid>, IAggregateRoot, ISoftDeletable
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsDeleted { get; set; }
        public Guid UserRoleId { get; set; }
        public UserRole UserRole { get; set; }
        public Tenant Tenant { get; set; }

        public User() : base()
        {

        }

        public User(string username, string passwordHash, string email, string firstname, string lastname, Guid userRoleId, Guid tenantId) : base(Guid.NewGuid())
        {
            Username = username;
            PasswordHash = passwordHash;
            Email = email;
            FirstName = firstname;
            LastName = lastname;
            UserRoleId = userRoleId;
            TenantId = tenantId;
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
