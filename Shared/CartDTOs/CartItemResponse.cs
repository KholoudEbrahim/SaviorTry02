using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CartDTOs
{
    public record CartItemResponse
    {
        public int CartID { get; set; }
        public int MedicineID { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string PriceType { get; set; } = "Strip"; 

        public decimal TotalPrice => Quantity * Price;
    }
}
