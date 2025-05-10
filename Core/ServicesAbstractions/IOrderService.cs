using Domain.Models.Enumerations;
using Domain.Models.OrderEntities;
using Shared.OrderDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesAbstractions
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(int userID, double userLatitude, double userLongitude, List<OrderItem> orderItems);
        Task<OrderResponse> GetOrderDetailsAsync(int orderID);
        Task<IEnumerable<OrderResponse>> GetPastOrdersAsync(int userID);
        Task ConfirmOrderAsync(int orderID);
        Task CancelOrderAsync(int orderID);
        decimal CalculateTotalPrice(List<OrderItem> orderItems);
        Task<IEnumerable<OrderResponse>> GetAllOrdersAsync();
    }
}
