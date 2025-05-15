using AutoMapper;
using Domain.Contracts;
using Domain.Models;
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
        private readonly NotificationService _notificationService;
        private readonly ILogger<OrderService> _logger;
        private const decimal FIXED_SHIPPING_PRICE = 20.00m;
        private readonly IOrderRepository _orderRepository;
        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, NotificationService notificationService, ILogger<OrderService> logger, IOrderRepository orderRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
            _logger = logger;
            _orderRepository = orderRepository;
        }

        public async Task<int> CreateOrderAsync(int userID, double userLatitude, double userLongitude, List<OrderItem> orderItems)
        {
            _logger.LogInformation($"Creating order for user {userID}");

            if (userLatitude < -90 || userLatitude > 90 || userLongitude < -180 || userLongitude > 180)
                throw new ArgumentException("Invalid latitude or longitude values.");

            var firstItem = orderItems.FirstOrDefault();
            if (firstItem == null)
                throw new ArgumentException("Order items cannot be empty.");

            // Get any pharmacy (assuming all pharmacies have all medicines)
            var pharmacies = await _unitOfWork.Pharmacies.GetAllAsync();
            var pharmacy = pharmacies.FirstOrDefault();
            if (pharmacy == null)
                throw new Exception("No pharmacy found.");

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
                OrderItems = orderItems,
                PharmacyId = pharmacy?.Id 
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
            try
            {

                var deliveryPerson = _unitOfWork.DeliveryPersons
                    .GetQueryable()
                    .FirstOrDefault();

                if (deliveryPerson != null)
                {
                    return deliveryPerson;
                }

                var seedingBasePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Seeding");
                var deliveryFilePath = Path.Combine(seedingBasePath, "deliveries.json");

                if (!File.Exists(deliveryFilePath))
                {
                    throw new FileNotFoundException($"Delivery persons file not found at: {deliveryFilePath}");
                }

                var jsonData = File.ReadAllText(deliveryFilePath);
                var deliveryPersons = JsonSerializer.Deserialize<List<DeliveryPerson>>(jsonData);

                return deliveryPersons?.FirstOrDefault() ??
                    throw new Exception("No delivery persons available");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning delivery person");
                throw;
            }
        }

        public async Task<OrderResponse> GetOrderDetailsAsync(int orderID)
        {
            var order = await _orderRepository.GetOrderWithDetailsAsync(orderID);
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
            var orders = await _orderRepository.GetPastOrdersAsync(userID);
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

        public async Task<IEnumerable<OrderResponse>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllUsersOrdersAsync();
            return _mapper.Map<IEnumerable<OrderResponse>>(orders);
        }
        //public async Task<IEnumerable<OrderResponse>> GetAllOrdersAsync()
        //{
        //    var orders = await _unitOfWork.Orders.GetAllAsync();
        //    return _mapper.Map<IEnumerable<OrderResponse>>(orders);
        //}
    }
}
//GetAllUsersOrdersAsync