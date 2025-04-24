using AutoMapper;
using Domain.Contracts;
using Domain.Models.Enumerations;
using Domain.Models.OrderEntities;
using ServicesAbstractions;
using Shared.OrderDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
        }

     
            private const decimal FIXED_SHIPPING_PRICE = 20.00m; 

            public async Task<int> CreateOrderAsync(int userID, double userLatitude, double userLongitude,
                                                  List<OrderItem> orderItems)
            {
                
                if (userLatitude < -90 || userLatitude > 90 || userLongitude < -180 || userLongitude > 180)
                {
                    throw new ArgumentException("Invalid latitude or longitude values.");
                }

            
                var firstItem = orderItems.FirstOrDefault();
                if (firstItem == null) throw new Exception("Order items cannot be empty");

                var supply = await _unitOfWork.Supplies.FindAsync(s =>
                    s.MedicineId == firstItem.MedicineID);
                var pharmacyId = supply.FirstOrDefault()?.PharmacyId;

                if (!pharmacyId.HasValue) throw new Exception("Could not determine pharmacy");

                var pharmacy = await _unitOfWork.Pharmacies.GetByIdAsync(pharmacyId.Value);
                if (pharmacy == null) throw new Exception("Pharmacy not found");

       
                var duration = CalculateDuration(
                    userLatitude,
                    userLongitude,
                    pharmacy.Latitude,
                    pharmacy.Longitude
                );

                decimal medicinesSubtotal = orderItems.Sum(item => item.Price * item.Quantity);
                decimal totalPrice = medicinesSubtotal + FIXED_SHIPPING_PRICE;

         
                var order = new Order
                {
                    UserID = userID,
                    UserLatitude = userLatitude,
                    UserLongitude = userLongitude,
                    Duration = duration,
                    PaymentWay = PaymentForOrder.CASH_ON_DELIVERY, 
                    ShippingPrice = FIXED_SHIPPING_PRICE,
                    TotalPrice = totalPrice,
                    OrderDate = DateTime.UtcNow,
                    OrderItems = orderItems
                };

             
                if (!pharmacy.HasDelivery)
                {
                    var deliveryPerson = AssignDeliveryPerson(pharmacy.Latitude, pharmacy.Longitude);
                    order.DeliveryName = deliveryPerson.Name;
                    order.DeliveryPhone = deliveryPerson.Phone;
                }

                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.CompleteAsync();

                return order.Id;
            }

        // Get Order Details
        public async Task<OrderResponse> GetOrderDetailsAsync(int orderID)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderID);
            if (order == null) throw new Exception("Order not found");

            var response = new OrderResponse
            {
                OrderID = order.Id,
                UserLatitude = order.UserLatitude,
                UserLongitude = order.UserLongitude,
                Duration = order.Duration ?? TimeSpan.Zero,
                PaymentWay = order.PaymentWay,
                DeliveryName = order.DeliveryName,
                DeliveryPhone = order.DeliveryPhone,
                OrderDate = order.OrderDate,
                MedicinesSubtotal = order.OrderItems.Sum(i => i.Price * i.Quantity),
                ShippingPrice = order.ShippingPrice,
                TotalPrice = order.TotalPrice,
                OrderItems = _mapper.Map<List<OrderItemResponse>>(order.OrderItems)
            };

            return response;
        }

        // Get Past Orders for a User
        public async Task<IEnumerable<OrderResponse>> GetPastOrdersAsync(int userID)
        {
            var orders = await _unitOfWork.Orders.FindAsync(o => o.UserID == userID);

            // Use AutoMapper to map List<Order> to List<OrderResponse>
            return _mapper.Map<IEnumerable<OrderResponse>>(orders);
        }

        // Confirm an Order
        public async Task ConfirmOrderAsync(int orderID)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderID);
            if (order == null) throw new Exception("Order not found");
            // Implement confirmation logic here
       
            order.OrderDate = DateTime.UtcNow;

            await _unitOfWork.CompleteAsync();
            await _notificationService.SendOrderConfirmationEmailAsync(order.UserID, order.Id);
        }

        // Cancel an Order
        public async Task CancelOrderAsync(int orderID)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderID);
            if (order == null) throw new Exception("Order not found");
            // Implement cancellation logic here
            await _unitOfWork.CompleteAsync();
        }

        // Calculate Total Price
        public decimal CalculateTotalPrice(List<OrderItem> orderItems)
        {
            return orderItems.Sum(item => item.Price * item.Quantity);
        }

        // Calculate Duration using Haversine Formula
        private TimeSpan CalculateDuration(double userLatitude, double userLongitude, double pharmacyLatitude, double pharmacyLongitude)
        {
            double earthRadiusKm = 6371;
            var dLat = ToRadians(pharmacyLatitude - userLatitude);
            var dLon = ToRadians(pharmacyLongitude - userLongitude);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(userLatitude)) * Math.Cos(ToRadians(pharmacyLatitude)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distanceKm = earthRadiusKm * c;

            if (distanceKm <= 5)
            {
                return TimeSpan.FromMinutes(15); // Close distance: 15 minutes
            }
            else if (distanceKm <= 10)
            {
                return TimeSpan.FromMinutes(30); // Medium distance: 30 minutes
            }
            else
            {
                return TimeSpan.FromHours(1); // Far distance: 1 hour
            }
        }

        // Convert Degrees to Radians
        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        // Assign Delivery Person based on proximity
        private DeliveryPerson AssignDeliveryPerson(double pharmacyLatitude, double pharmacyLongitude)
        {
            var deliveries = LoadJson<List<DeliveryPerson>>("Data/Deliveries/deliveries.json");

            // Calculate the distance between the pharmacy and each delivery person
            var closestDelivery = deliveries
                .Select(d => new
                {
                    DeliveryPerson = d,
                    Distance = CalculateDistance(pharmacyLatitude, pharmacyLongitude, d.Latitude, d.Longitude)
                })
                .OrderBy(x => x.Distance)
                .FirstOrDefault();

            if (closestDelivery == null) throw new Exception("No delivery person available for this area.");

            return closestDelivery.DeliveryPerson;
        }

        // Helper method to calculate distance using Haversine formula
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double earthRadiusKm = 6371;
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return earthRadiusKm * c;
        }

        // Load JSON Data
        private T LoadJson<T>(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(json);
        }
 

    }
}
