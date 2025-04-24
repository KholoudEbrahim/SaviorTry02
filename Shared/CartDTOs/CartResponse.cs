using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CartDTOs
{
    public class CartResponse
    {
        public int UserID { get; set; }
        public List<CartItemResponse> Items { get; set; } = new List<CartItemResponse>();
        public decimal TotalPrice => Items.Sum(item => item.TotalPrice);
    }
}
