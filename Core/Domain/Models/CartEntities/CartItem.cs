using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.CartEntities
{
    public class CartItem
    {
        public int MedicineID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        // Foreign key for Cart
        public int CartId { get; set; } // This represents the Id of the associated Cart

        // Navigation properties
        public Medicine Medicine { get; set; } = null!;
        public virtual Cart Cart { get; set; } = null!;
    }
}
