using Domain.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.OrderDTOs
{
    public class OrderResponse
    {
        public int OrderID { get; set; }
        public double UserLatitude { get; set; }
        public double UserLongitude { get; set; }
        public TimeSpan Duration { get; set; }
        public PaymentForOrder PaymentWay { get; set; }
        public string? DeliveryName { get; set; }
        public string? DeliveryPhone { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal MedicinesSubtotal { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItemResponse> OrderItems { get; set; } = new List<OrderItemResponse>();
    }
}
