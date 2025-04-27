using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CartDTOs
{
    public class CartItemRequest
    {
        public int UserID { get; set; }
        public int MedicineID { get; set; }
        public int Quantity { get; set; }
        public string PriceType { get; set; } = "Strip";
    }
}
