using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.CartEntities
{
    public class CartItem
    {
        [Key]

        public int CartItemID { get; set; }
        public int MedicineID { get; set; }
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string PriceType { get; set; } = "Strip";

        // Foreign key for Cart
        public int CartId { get; set; }

        public Medicine Medicine { get; set; } = null!;
        public virtual Cart Cart { get; set; } = null!;
    }
}
