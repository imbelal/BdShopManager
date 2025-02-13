﻿using Common.Entities;
using Domain.Enums;

namespace Domain.Entities
{
    public class OrderDetail : AuditableTenantEntityBase<Guid>
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public ProductUnit Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public virtual Order Order { get; set; }
        public Tenant Tenant { get; set; }

        public OrderDetail() : base()
        {

        }

        public OrderDetail(Guid orderId, Guid productId, int quantity, ProductUnit unit, decimal unitPrice) : base(Guid.NewGuid())
        {
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            Unit = unit;
            UnitPrice = unitPrice;
            TotalPrice = unitPrice * quantity;
        }
    }
}
