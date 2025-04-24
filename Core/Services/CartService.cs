using AutoMapper;
using Domain.Contracts;
using Domain.Models.CartEntities;
using ServicesAbstractions;
using Shared.CartDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Add Medicine to Cart
        public async Task AddToCartAsync(int userID, int medicineID, int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.");
            }

            var cart = await GetOrCreateCartAsync(userID);

            var existingItem = cart.Items.FirstOrDefault(item => item.MedicineID == medicineID);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    MedicineID = medicineID,
                    Quantity = quantity
                });
            }

            await _unitOfWork.CompleteAsync();
        }

        // Remove Medicine from Cart
        public async Task RemoveFromCartAsync(int userID, int medicineID)
        {
            var cart = await GetCartEntityAsync(userID);
            if (cart != null)
            {
                var itemToRemove = cart.Items.FirstOrDefault(item => item.MedicineID == medicineID);
                if (itemToRemove != null)
                {
                    cart.Items.Remove(itemToRemove);
                    await _unitOfWork.CompleteAsync();
                }
            }
        }

        // Get Cart Response for a User (for controller/view)
        public async Task<CartResponse> GetCartAsync(int userID)
        {
            var cart = await GetCartEntityAsync(userID);
            if (cart == null || cart.Items == null || !cart.Items.Any())
                return null;

            return _mapper.Map<CartResponse>(cart);
        }

        // Helper: Get existing cart or create one if it doesn't exist
        private async Task<Cart> GetOrCreateCartAsync(int userID)
        {
            var cart = await GetCartEntityAsync(userID);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userID,
                    Items = new List<CartItem>()
                };
                await _unitOfWork.Carts.AddAsync(cart);
            }
            return cart;
        }

        // Helper: Get Cart Entity only (internal use)
        private async Task<Cart> GetCartEntityAsync(int userID)
        {
            var carts = await _unitOfWork.Carts.FindAsync(c => c.UserId == userID);
            return carts.FirstOrDefault();
        }
    }

}
