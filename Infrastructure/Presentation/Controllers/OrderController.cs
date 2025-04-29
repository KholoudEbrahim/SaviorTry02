using Domain.Contracts;
using Domain.Models.Enumerations;
using Domain.Models.OrderEntities;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstractions;
using Shared.CartDTOs;
using Shared.OrderDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IOrderService orderService, ICartService cartService, IUnitOfWork unitOfWork)
        {
            _orderService = orderService;
            _cartService = cartService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CartCheckoutRequest request)
        {
            try
            {
                if (request == null || request.UserID <= 0)
                    return BadRequest("Invalid request data.");

          
                var user = await _unitOfWork.Users.GetByIdAsync(request.UserID);
                if (user == null)
                    return BadRequest("User not found.");

    
                Console.WriteLine($"Attempting checkout for user {request.UserID}");
                Console.WriteLine($"Coordinates: {request.UserLatitude}, {request.UserLongitude}");

                var cart = await _cartService.GetCartAsync(request.UserID);
                if (cart == null || !cart.Items.Any())
                {
                    Console.WriteLine($"Cart for user {request.UserID} is empty");
                    return BadRequest("Cart is empty.");
                }

                Console.WriteLine($"Cart contains {cart.Items.Count} items");

                var orderItems = cart.Items.Select(item => new OrderItem
                {
                    MedicineID = item.MedicineID,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList();

                Console.WriteLine($"Cart Items:");
                foreach (var item in cart.Items)
                {
                    Console.WriteLine($"- MedicineID: {item.MedicineID}, Quantity: {item.Quantity}, Price: {item.Price}");
                }


                Console.WriteLine($"Order Items:");
                foreach (var item in orderItems)
                {
                    Console.WriteLine($"- MedicineID: {item.MedicineID}, Quantity: {item.Quantity}, Price: {item.Price}");
                }

                var orderID = await _orderService.CreateOrderAsync(
                    request.UserID,
                    request.UserLatitude,
                    request.UserLongitude,
                    orderItems
                );
              

                Console.WriteLine($"Order created with ID: {orderID}");

                var newOrder = await _orderService.GetOrderDetailsAsync(orderID);

                return CreatedAtAction(nameof(GetOrderDetails), new { id = orderID }, new
                {
                    OrderId = orderID,
                    Message = "Order created successfully",
                    OrderDetails = newOrder
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during checkout: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your order.");
            }
        }
        [HttpPost("{orderId}/confirm")]
        public async Task<IActionResult> ConfirmOrder(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                return NotFound($"Order with ID {orderId} does not exist");

            await _orderService.ConfirmOrderAsync(orderId);
            var confirmedOrder = await _orderService.GetOrderDetailsAsync(orderId);

            return Ok(new
            {
                Message = "Order confirmed successfully",
                Order = confirmedOrder
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            var order = await _orderService.GetOrderDetailsAsync(id);
            if (order == null)
                return NotFound($"Order with ID {id} not found.");

            return Ok(order);
        }

        [HttpGet("past-orders/{userID}")]
        public async Task<IActionResult> GetPastOrders(int userID)
        {
            var orders = await _orderService.GetPastOrdersAsync(userID);
            return Ok(orders);
        }
    }
}