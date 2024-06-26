﻿using Common.Entities;
using Common.Entities.Interfaces;
using Domain.Dtos;

namespace Domain.Entities
{
    public class Order : AuditableEntityBase, IAggregateRoot, ISoftDeletable
    {
        private List<OrderDetail> orderDetails = new();
        public Guid CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalPaid { get; set; }
        public string Remark { get; set; }
        public bool IsDeleted { get; set; } = false;

        public IReadOnlyCollection<OrderDetail> OrderDetails
        {
            get => orderDetails;
        }

        public Order(Guid customerId, decimal totalPrice, decimal totalPaid, string remark)
        {
            CustomerId = customerId;
            TotalPrice = totalPrice;
            TotalPaid = totalPaid;
            Remark = remark;
        }

        public static Order CreateOrderWithDetails(Guid customerId, decimal totalPrice, decimal totalPaid, string remark, List<OrderDetailsDto> orderDetailDtos)
        {
            Order newOrder = new(customerId, totalPrice, totalPaid, remark);
            newOrder.orderDetails.AddRange(
                orderDetailDtos.Select(orderDetail => new OrderDetail(newOrder.Id,
                orderDetail.ProductId,
                orderDetail.Quantity,
                orderDetail.Unit,
                orderDetail.UnitPrice)).ToList()
            );

            return newOrder;
        }
    }
}
