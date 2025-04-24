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
        public OrderController(IOrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }

        // Checkout and Create an Order
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CartCheckoutRequest request)
        {
            try
            {
              
                if (request == null || request.UserID <= 0)
                {
                    return BadRequest("Invalid request data.");
                }
                              var cart = await _cartService.GetCartAsync(request.UserID);
                if (cart == null || !cart.Items.Any())
                {
                    return BadRequest("Cart is empty.");
                }

   
                var orderItems = cart.Items.Select(item => new OrderItem
                {
                    MedicineID = item.MedicineID,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList();


                var orderID = await _orderService.CreateOrderAsync(
                    request.UserID,
                    request.UserLatitude,
                    request.UserLongitude,
                    orderItems
                );

                return CreatedAtAction(nameof(GetOrderDetails), new { id = orderID }, new
                {
                    OrderId = orderID,
                    Message = "Order created successfully. Payment method: Cash on delivery"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpPost("{orderId}/confirm")]
        public async Task<IActionResult> ConfirmOrder(int orderId)
        {
            try
            {
                await _orderService.ConfirmOrderAsync(orderId);
                return Ok(new { Message = "Order confirmed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    

        // Get Order Details
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            try
            {
                var order = await _orderService.GetOrderDetailsAsync(id);
                if (order == null) return NotFound();
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        // Get Past Orders for a User
        [HttpGet("past-orders/{userID}")]
        public async Task<IActionResult> GetPastOrders(int userID)
        {
            try
            {
                var orders = await _orderService.GetPastOrdersAsync(userID);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");


            }
        }
    }
}
