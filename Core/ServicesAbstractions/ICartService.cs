using Domain.Models.CartEntities;
using Shared.CartDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesAbstractions
{
    public interface ICartService
    {
        Task AddToCartAsync(int userID, int medicineID, int quantity);
        Task RemoveFromCartAsync(int userID, int medicineID);
        Task<CartResponse> GetCartAsync(int userID); 
    }
}
