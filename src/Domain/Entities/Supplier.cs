﻿using Common.Entities;
using Common.Entities.Interfaces;

namespace Domain.Entities
{
    public class Supplier : AuditableTenantEntityBase<Guid>, IAggregateRoot, ISoftDeletable
    {
        public string Name { get; private set; }
        public string Details { get; private set; }
        public string ContactNo { get; private set; }
        public bool IsDeleted { get; set; } = false;
        public Tenant Tenant { get; set; }

        public Supplier() : base()
        {

        }

        public Supplier(string name, string details, string contactNo) : base(Guid.NewGuid())
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
