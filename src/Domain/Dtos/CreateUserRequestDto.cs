﻿namespace Domain.Dtos
{
    public class CreateUserRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid UserRoleId { get; set; }
        public Guid TenantId { get; set; }
    }
}
