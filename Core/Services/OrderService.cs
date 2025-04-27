using AutoMapper;
using Domain.Contracts;
using Domain.Models.Enumerations;
using Domain.Models.OrderEntities;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<OrderService> _logger;

        private const decimal FIXED_SHIPPING_PRICE = 20.00m;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService, ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<int> CreateOrderAsync(int userID, double userLatitude, double userLongitude, List<OrderItem> orderItems)
        {
            _logger.LogInformation($"Creating order for user {userID}");

            if (userLatitude < -90 || userLatitude > 90 || userLongitude < -180 || userLongitude > 180)
                throw new ArgumentException("Invalid latitude or longitude values.");

            var firstItem = orderItems.FirstOrDefault();
            if (firstItem == null)
                throw new ArgumentException("Order items cannot be empty.");

            var supply = await _unitOfWork.Supplies.FindAsync(s => s.MedicineId == firstItem.MedicineID);
            var pharmacyId = supply.FirstOrDefault()?.PharmacyId;

            if (!pharmacyId.HasValue)
                throw new Exception("Could not determine pharmacy.");

            var pharmacy = await _unitOfWork.Pharmacies.GetByIdAsync(pharmacyId.Value);
            if (pharmacy == null)
                throw new Exception("Pharmacy not found.");

            var duration = CalculateDuration(userLatitude, userLongitude, pharmacy.Latitude, pharmacy.Longitude);

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
        private DeliveryPerson AssignDeliveryPerson(double pharmacyLatitude, double pharmacyLongitude)
        {

            var jsonData = File.ReadAllText("deliveries.json");

            var deliveryPersons = JsonSerializer.Deserialize<List<DeliveryPerson>>(jsonData);

            if (deliveryPersons == null || deliveryPersons.Count == 0)
                throw new Exception("No delivery persons available.");
            var selectedDeliveryPerson = deliveryPersons.First();

            return selectedDeliveryPerson;
        }
        public async Task<OrderResponse> GetOrderDetailsAsync(int orderID)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderID);
            if (order == null)
                return null;

            return new OrderResponse
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
        }

        public async Task<IEnumerable<OrderResponse>> GetPastOrdersAsync(int userID)
        {
            var orders = await _unitOfWork.Orders.FindAsync(o => o.UserID == userID);
            return _mapper.Map<IEnumerable<OrderResponse>>(orders);
        }

        public async Task ConfirmOrderAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");

            order.OrderDate = DateTime.UtcNow;
            await _unitOfWork.CompleteAsync();

            await _notificationService.SendOrderConfirmationEmailAsync(order.UserID, order.Id);
        }

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
                return TimeSpan.FromMinutes(15);

            if (distanceKm <= 15)
                return TimeSpan.FromMinutes(30);

            return TimeSpan.FromMinutes(45);
        }

        private double ToRadians(double angle) => Math.PI * angle / 180.0;



        public async Task CancelOrderAsync(int orderID)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderID);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {orderID} not found.");

            _unitOfWork.Orders.Remove(order);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Order {orderID} has been cancelled.");
        }
        public decimal CalculateTotalPrice(List<OrderItem> orderItems)
        {
            if (orderItems == null || !orderItems.Any())
                throw new ArgumentException("Order items cannot be empty.");

            decimal medicinesSubtotal = orderItems.Sum(item => item.Price * item.Quantity);
            return medicinesSubtotal + FIXED_SHIPPING_PRICE;
        }

    }
}