using Microsoft.AspNetCore.Mvc;
using ServicesAbstractions;
using Shared.CartDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // Add Medicine to Cart
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] CartItemRequest request)
        {
            try
            {
                if (request == null || request.UserID <= 0 || request.MedicineID <= 0 || request.Quantity <= 0)
                {
                    return BadRequest("Invalid input data.");
                }

                await _cartService.AddToCartAsync(request.UserID, request.MedicineID, request.Quantity, request.PriceType);
                return Ok("Item added to cart successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid input: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Remove Medicine from Cart
        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromCart([FromBody] CartItemRequest request)
        {
            try
            {
                await _cartService.RemoveFromCartAsync(request.UserID, request.MedicineID, request.PriceType);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Get Cart for a User
        [HttpGet("{userID}")]
        public async Task<IActionResult> GetCart(int userID)
        {
            try
            {
                var cart = await _cartService.GetCartAsync(userID);
                if (cart == null) return NotFound("Cart not found.");
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpDelete("clear-all")]
        public async Task<IActionResult> ClearCart([FromBody] ClearCartRequest request)
        {
            try
            {
                
                if (request == null || request.UserID <= 0)
                {
                    return BadRequest("Invalid user ID.");
                }

                
                await _cartService.ClearCartAsync(request.UserID);

                return Ok(new { Message = "Cart cleared successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
