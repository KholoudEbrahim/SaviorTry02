using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.OrderEntities
{
    public class OrderItem : BaseEntity
    {
        public int OrderID { get; set; }
        public int MedicineID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        // Navigation Properties
        [ForeignKey("OrderID")]
        public virtual Order Order { get; set; } = null!;

        [ForeignKey("MedicineID")]
        public virtual Medicine Medicine { get; set; } = null!;
    }
}
